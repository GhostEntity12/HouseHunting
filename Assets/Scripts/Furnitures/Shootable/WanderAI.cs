using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour
{
	enum ThresholdLevels { Level0, Level1, Level2, Level3 }
	const float aiEntityDistance = 100f; // This distance will be used to determine whether or not the AI should run or not.

	private FurnitureSO info;

	private Shootable shootable;
	private ThresholdLevels alertLevel = ThresholdLevels.Level0;
	private bool isRelaxed;
	private float relaxTimer = 0f;
	private NavMeshAgent agent;
	private readonly Queue<SoundAlert> sounds = new();
	private Transform player;
	private float alertness = 0;
	private AIBehaviour activeBehaviour;

	public float Alertness
	{
		get => alertness;
		private set => alertness = Mathf.Clamp(value, 0, 100);
	}

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		shootable = GetComponent<Shootable>();
	}

	private void Start()
	{
		// Cache some values
		player = HuntingManager.Instance.Player;
		info = shootable.FurnitureSO;
		activeBehaviour = Instantiate(info.threshold0Behaviour);

		// Populate the speed from the stats
		agent.speed = info.speed;
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
		ProcessSounds(out SoundAlert? sound);

		// Change alertness based on whether the player is visible
		UpdateSightAlertness(canSeePlayer);

		// Bundle information to pass to behaviours
		Knowledge k = new(transform, player.position, info, agent, sound, canSeePlayer);

		CheckTransitions(k);

		activeBehaviour.Act(ref k);

	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (info && info.senses.Length == 0 || !Application.isPlaying) return;
		foreach (ViewConeSO cone in info.senses)
		{
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			cone.DebugDraw(transform, player.position, 0.5f);
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
			cone.DebugDraw(transform, player.position, 0.2f);
		}
	}
#endif

	private void CheckTransitions(Knowledge k)
	{
		switch (alertLevel)
		{
			case ThresholdLevels.Level0:
				if (Alertness > info.alertnessThreshold1)
				{
					Transition(info.threshold1Behaviour, k, ThresholdLevels.Level1);
				}
				break;
			case ThresholdLevels.Level1:
				if (Alertness == 0)
				{
					Transition(info.threshold0Behaviour, k, ThresholdLevels.Level0);
				}
				else if (Alertness >= info.alertnessThreshold2)
				{
					Transition(info.threshold2Behaviour, k, ThresholdLevels.Level2);
				}
				break;
			case ThresholdLevels.Level2:
				if (Alertness < info.alertnessThreshold1)
				{
					Transition(info.threshold1Behaviour, k, ThresholdLevels.Level1);
				}
				else if (Alertness >= info.alertnessThreshold3)
				{
					Transition(info.threshold3Behaviour, k, ThresholdLevels.Level3);
				}
				break;
			case ThresholdLevels.Level3:
				if (Alertness < info.alertnessThreshold1)
				{
					Transition(info.threshold0Behaviour, k, ThresholdLevels.Level0);
				}
				break;
		}
	}

	private void Transition(AIBehaviour newBehaviour, Knowledge knowledge, ThresholdLevels newAlertLevel)
	{
		// Call the behaviour's exit
		activeBehaviour.Exit(ref knowledge);
		// Destroy the old behaviour
		Destroy(activeBehaviour);
		// Make a copy of the new behaviour
		activeBehaviour = Instantiate(newBehaviour);
		// Call the new behaviour's entry
		activeBehaviour.Entry(ref knowledge);
		// Update the state
		alertLevel = newAlertLevel;
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

		foreach (ViewConeSO cone in info.senses)
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
		Debug.DrawLine(fleeTo, fleeTo + (Vector3.up * 10), Color.red, 15);

		return fleeTo;
	}

	public static bool RandomPoint(Vector3 center, float range, out Vector3 result)
	{
		Vector3 randomPoint = center + (Random.insideUnitSphere * range); //random point in a sphere 
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
			Alertness += Time.deltaTime * info.sightAlertnessRate;
			isRelaxed = false;
			relaxTimer = info.timeBeforeDecay;
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
			Alertness -= Time.deltaTime * info.alertnessDecayRate;
	}

	/// <summary>
	/// Iterates through the queue of sounds and adds their volume to the furniture's alertness 
	/// </summary>
	/// <param name="loudestSound">The sound with the largest volume</param>
	private void ProcessSounds(out SoundAlert? loudestSound)
	{
		if (sounds.Count == 0)
		{
			loudestSound = null;
			return;
		}
		else
		{
			loudestSound = new(Vector3.zero, 0);
			isRelaxed = false;
			relaxTimer = info.timeBeforeDecay;
			while (sounds.Count > 0)
			{
				SoundAlert sound = sounds.Dequeue();
				if (sound.volume > ((SoundAlert)loudestSound).volume)
				{
					loudestSound = sound;
				}
				Alertness += sound.volume;
			}
		}
	}

	/// <summary>
	/// Adds a sound to be processed on the next frame
	/// </summary>
	/// <param name="sound"></param>
	public void EnqueueSound(SoundAlert sound) => sounds.Enqueue(sound);
}