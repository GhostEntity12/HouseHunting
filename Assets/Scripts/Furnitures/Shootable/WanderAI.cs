using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class WanderAI : MonoBehaviour
{
	enum AlertLevels { None, Level1, Level2, Level3 }
										 //enum PathfindingState { Fleeing, Chasing, Investigating, Looking, Wandering }
	const float aiEntityDistance = 100f; // This distance will be used to determine whether or not the AI should run or not.

	[SerializeField] private FurnitureSO stats;
	[SerializeField] private Behaviour threshold0Behaviour;
	[SerializeField] private Behaviour threshold1Behaviour;
	[SerializeField] private Behaviour threshold2Behaviour;
	[SerializeField] private Behaviour threshold3Behaviour;

	private Shootable shootable;
	private Canvas alertCanvas;
	private AlertLevels currentBehaviour = AlertLevels.None;
	private bool isRelaxed;
	private float relaxTimer = 0f;
	private Vector3? dangerPosition = null;
	private NavMeshAgent agent;
	private readonly Queue<SoundAlert> sounds = new();
	private Transform player;
	private float alertness = 0;

	public float Alertness
	{
		get => alertness;
		private set => alertness = Mathf.Clamp(value, 0, 100);
	}
	public bool IsAggressive => stats.behavior != AIType.Prey;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		shootable = GetComponent<Shootable>();
		alertCanvas = GetComponentInChildren<Canvas>();
	}

	private void Start()
	{
		// Cache some values
		player = HuntingManager.Instance.Player;
		stats = shootable.FurnitureSO;

		// Populate the speed from the stats
		agent.speed = stats.speed;
	}

	private void Update()
	{
		if (shootable.IsDead)
		{
			agent.isStopped = true;
			return;
		}

		// Culling behaviour if outside of entity distance
		if (!player || Vector3.Distance(player.position, transform.position) > aiEntityDistance)
			return;

		// Cache player visibility
		bool canSeePlayer = HasLineOfSight();

		// Deal with sounds
		ProcessSounds(out SoundAlert sound);

		// Change alertness based on whether the player is visible
		UpdateSightAlertness(canSeePlayer);

		// Bundle information to pass to behaviours
		Knowledge k = new(transform.position, transform.forward, player.position, dangerPosition, stats, agent, sound, canSeePlayer);

		// State machine
		switch (currentBehaviour)
		{
			case AlertLevels.None:
				Threshold0(k);
				break;
			case AlertLevels.Level1:
				Threshold1(k);
				break;
			case AlertLevels.Level2:
				Threshold2(k);
				break;
			case AlertLevels.Level3:
				Threshold3(k);
				break;
		}

		// Update the cached danger position in case the behaviours changed it.
		dangerPosition = k.dangerPosition;

		// This is debug
		if (Input.GetKeyDown(KeyCode.T))
		{
			ToggleDoesRotate();
		}

	}

	private void OnDrawGizmos()
	{
		if (stats.senses.Length == 0 || !Application.isPlaying) return;
		foreach (ViewConeSO cone in stats.senses)
		{
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			cone.DebugDraw(transform, player.position, 0.5f);
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
			cone.DebugDraw(transform, player.position, 0.2f);
		}
	}

	private void Threshold0(Knowledge knowledge)
	{
		// Update behaviour level where appropriate
		if (Alertness > stats.alertnessThreshold1)
			TransitionToThreshold1();

		threshold0Behaviour.Act(ref knowledge);
	}

	private void Threshold1(Knowledge knowledge)
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case 0:
				TransitionToThreshold0();
				break;
			case float a when a >= stats.alertnessThreshold2:
				TransitionToThreshold2();
				break;
		}

		threshold1Behaviour.Act(ref knowledge);
	}

	private void Threshold2(Knowledge knowledge)
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case float a when a >= stats.alertnessThreshold1:
				TransitionToThreshold1();
				break;
			case float a when a >= stats.alertnessThreshold3:
				TransitionToThreshold3();
				break;
		}

		threshold2Behaviour.Act(ref knowledge);
	}

	private void Threshold3(Knowledge knowledge)
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case float a when a < stats.alertnessThreshold1:
				TransitionToThreshold1();
				break;
			case float a when a > stats.alertnessThreshold3:
				TransitionToThreshold3();
				break;
		}

		threshold3Behaviour.Act(ref knowledge);
	}

	void TransitionToThreshold0()
	{
		currentBehaviour = AlertLevels.None;
	}

	private void TransitionToThreshold1()
	{
		// Clear the path
		agent.ResetPath();
		// Clear any knowledge of where the player is
		dangerPosition = null;
		// Seize control of rotation
		agent.updateRotation = false;
		// Ensure speed is correct
		agent.speed = stats.speed;
		currentBehaviour = AlertLevels.Level1;
	}
	
	private void TransitionToThreshold2()
	{
		// Release control of rotation
		agent.updateRotation = true;
		agent.speed = stats.speed;
		currentBehaviour = AlertLevels.Level2;
	}
	
	private void TransitionToThreshold3()
	{
		agent.speed = stats.speed * 2;
		currentBehaviour = AlertLevels.Level3;
	}

	/// <summary>
	/// Calculate whether the furniture can see the player
	/// </summary>
	/// <returns></returns>
	private bool HasLineOfSight()
	{
		// Treat the player's position as the center of it's body
		// TODO: alter if the player is crouching?
		Vector3 playerCenter = player.position + Vector3.up;

		// Get the direction to the player
		Vector3 targetDirection = playerCenter - transform.position;

		foreach (ViewConeSO cone in stats.senses)
		{
			// If the player is in range and within angle, do a raycast, check if the hit is the player
			// NOTE: This currently casts against everything in the scene. May need to add a layermask to prevent hits with transparent objects.
			if (cone.InCone(transform, playerCenter) &&
				Physics.Raycast(transform.position, targetDirection, out RaycastHit hit, 100, ~(1 << 14)))
			{
				if (hit.transform.CompareTag("Player"))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Returns a point distance units away in the opposite direction from fleeFrom
	/// </summary>
	/// <param name="fleeFrom"></param>
	/// <param name="currentPosition"></param>
	/// <param name="distance"></param>
	public static Vector3 FindFleePoint(Vector3 fleeFrom, Vector3 currentPosition, float distance = 3)
	{
		// Flatten the fleeFrom to be level with the AI
		fleeFrom.y = currentPosition.y;

		// Calculate the position to flee to
		Vector3 fleeTo = currentPosition + ((currentPosition - fleeFrom).normalized * distance);
		// Debug draws
		Debug.DrawLine(fleeFrom, currentPosition, Color.cyan, 15);
		Debug.DrawLine(currentPosition, fleeTo, Color.blue, 15);
		Debug.DrawLine(fleeTo, fleeTo + Vector3.up * 10, Color.red, 15);

		return fleeTo;
	}

	public static bool RandomPoint(Vector3 center, float range, out Vector3 result)
	{
		Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
		if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
		{
			//the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
			//or add a for loop like in the documentation
			result = hit.position;
			return true;
		}

		result = Vector3.zero;
		return false;
	}

	/// <summary>
	/// Updates the furniture's alertness based on whether it can see the player
	/// </summary>
	/// <param name="seesPlayer"></param>
	private void UpdateSightAlertness(bool seesPlayer)
	{
		// Can see player, don't relax and keep relax timer at max
		if (seesPlayer)
		{
			isRelaxed = false;
			relaxTimer = stats.timeBeforeDecay;
		}
		// Can't see player and but not relaxed yet
		else if (!isRelaxed)
		{
			relaxTimer -= Time.deltaTime;
			if (relaxTimer <= 0)
				isRelaxed = true;
		}
		// Decrease relaxTimer
		else
			Alertness -= Time.deltaTime * stats.alertnessDecayRate;
	}

	/// <summary>
	/// Iterates through the queue of sounds and adds their volume to the furniture's alertness 
	/// </summary>
	/// <param name="loudestSound">The sound with the largest volume</param>
	private void ProcessSounds(out SoundAlert loudestSound)
	{
		loudestSound = new(Vector3.zero, 0);
		while (sounds.Count > 0)
		{
			SoundAlert sound = sounds.Dequeue();
			if (sound.volume > loudestSound.volume)
			{
				loudestSound = sound;
			}
			Alertness += sound.volume;
		}
	}

	/// <summary>
	/// Adds a sound to be processed on the next frame
	/// </summary>
	/// <param name="sound"></param>
	public void EnqueueSound(SoundAlert sound) => sounds.Enqueue(sound);

	[ContextMenu("Toggle Does Rot")]
	public void ToggleDoesRotate() => agent.updateRotation = !agent.updateRotation;

}