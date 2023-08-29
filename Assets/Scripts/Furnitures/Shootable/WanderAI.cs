using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class WanderAI : MonoBehaviour
{
	const float aiEntityDistance = 100f; // This distance will be used to determine whether or not the AI should run or not.
										 //enum PathfindingState { Fleeing, Chasing, Investigating, Looking, Wandering }
	enum AlertLevels { None, Level1, Level2, Level3 }

	private Shootable shootable;
	private Canvas alertCanvas;
	//private PathfindingState pathfindingState = PathfindingState.Wandering;
	private AlertLevels currentBehaviour = AlertLevels.None;
	//private float timeSinceLastAttack = 0f;
	//private float timeSinceLastBump = 0f;
	private bool isRelaxed;
	private float relaxTimer = 0f;
	private Vector3? dangerPosition = null;

	//private readonly float fleeingSearchRange = 3.0f;
	//private readonly float investigateRadius = 2f;
	//private readonly float alertDistance = 30f;
	//private readonly float timePerBump = 1f; // This is used to determine if the AI pathfinding should recalculate itself, so that it can properly escape the player.

	private NavMeshAgent agent;
	//public float wanderRadius = 15f; // how far the AI can wander

	private readonly Queue<SoundAlert> sounds = new();
	private Transform player;
	private FurnitureSO stats;

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

		// State machine
		switch (currentBehaviour)
		{
			case AlertLevels.None:
				Threshold0();
				break;
			case AlertLevels.Level1:
				Threshold1(canSeePlayer);
				break;
			case AlertLevels.Level2:
				Threshold2(canSeePlayer, sound);
				break;
			case AlertLevels.Level3:
				Threshold3();
				break;
		}

		// This is debug
		if (Input.GetKeyDown(KeyCode.T))
		{
			ToggleDoesRotate();
		}
	}

	private void Threshold0()
	{
		// Update behaviour level where appropriate
		if (Alertness > 33)
			TransitionToThreshold1();

		Roam();
	}

	private void Threshold1(bool playerVisible)//, SoundAlert sound)
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case 0:
				TransitionToThreshold0();
				break;
			case > 66:
				TransitionToThreshold2();
				break;
		}

		if (playerVisible)
		{
			// Cache the player's position in case it loses sight of the player
			dangerPosition = player.position;

			// Look at the player
		}
		else if (dangerPosition == null)
		{
			// Hasn't yet seen and can't see the player in this state.
			// Unsure if this is going to be added
			//if (sound.volume > 0)
			//{
			//	dangerPosition = sound.position;
			//}
			// Look around randomly
		}
		else
		{
			// Look at dangerPosition
		}
	}


	private void Threshold2(bool playerVisible, SoundAlert sound)
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case < 33:
				TransitionToThreshold1();
				break;
			case 100:
				TransitionToThreshold3();
				break;
		}

		// Update dangerPosition if required
		if (playerVisible)
		{
			dangerPosition = player.position;
		}
		else if (sound.volume > 0)
		{
			dangerPosition = sound.position;
		}

		// If we reached the end of our current path or a new sound has been detected, generate a new path

	}

	private void Threshold3()
	{
		// Update behaviour level where appropriate
		switch (Alertness)
		{
			case < 33:
				TransitionToThreshold1();
				break;
			case 100:
				TransitionToThreshold3();
				break;
		}

		dangerPosition = player.position;
		if (agent.remainingDistance < 1 && RandomPoint(FindFleePoint((Vector3)dangerPosition), 2, out Vector3 fleeDestination))
		{
			agent.SetDestination(fleeDestination);
		}
	}

	void TransitionToThreshold0()
	{
		currentBehaviour = AlertLevels.None;
	}

	void TransitionToThreshold1()
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
	void TransitionToThreshold2()
	{
		// Release control of rotation
		agent.updateRotation = true;
		agent.speed = stats.speed;
		currentBehaviour = AlertLevels.Level2;
	}
	void TransitionToThreshold3()
	{
		agent.speed = stats.speed * 2;
		currentBehaviour = AlertLevels.Level3;
	}

	/// <summary>
	/// Gets the average sense distance
	/// </summary>
	/// <returns></returns>
	private float AverageSightDist() => stats.senses.Average(vc => vc.length);

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
	/// <param name="distance"></param>
	private Vector3 FindFleePoint(Vector3 fleeFrom, float distance = 3)
	{
		// Flatten the fleeFrom to be level with the AI
		fleeFrom.y = transform.position.y;

		// Calculate the position to flee to
		Vector3 fleeTo = transform.position + ((transform.position - fleeFrom).normalized * distance);
		// Debug draws
		Debug.DrawLine(fleeFrom, transform.position, Color.cyan, 15);
		Debug.DrawLine(transform.position, fleeTo, Color.blue, 15);
		Debug.DrawLine(fleeTo, fleeTo + Vector3.up * 10, Color.red, 15);

		return fleeTo;
	}

	private void Roam()
	{
		// If the agent is close to it's destination, choose a new one
		if (agent.remainingDistance < 1)
		{
			// Get a point in front of the furniture, and a random point
			// in a circle centered on that point, then navigate to it
			// This favours the AI moving generally forward
			if (RandomPoint(transform.position + transform.forward * 5, 3, out Vector3 newPointForward))
			{
				agent.SetDestination(newPointForward);
			}
			// If a point can't be found, then get a new point centered 
			// on the AI. This lets the AI choose a new random forward
			// An intemediary step could be added where it tries to reverse first
			else if (RandomPoint(transform.position, 10, out Vector3 newPoint))
			{
				agent.SetDestination(newPoint);
			}
		}
	}

	private bool RandomPoint(Vector3 center, float range, out Vector3 result)
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
	/// Adds a sound to be processed on the next frame
	/// </summary>
	/// <param name="sound"></param>
	public void EnqueueSound(SoundAlert sound) => sounds.Enqueue(sound);

	/// <summary>
	/// Iterates through the queue of sounds and adds their volume to the furniture's alertness 
	/// </summary>
	/// <param name="loudestSound">The sound with the largest volume</param>
	void ProcessSounds(out SoundAlert loudestSound)
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

	[ContextMenu("Toggle Does Rot")]
	public void ToggleDoesRotate() => agent.updateRotation = !agent.updateRotation;

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

	// TODO: Rework this
	//private void PredatorAI()
	//{
	//	float distance = Vector3.Distance(transform.position, player.transform.position);
	//
	//	if (distance <= 2f)
	//	{
	//		agent.isStopped = true;
	//		if (Alertness >= stats.alertnessThreshold3)
	//		{
	//			AttackPlayer();
	//		}
	//	}
	//	else
	//	{
	//		agent.isStopped = false;
	//		agent.SetDestination(player.transform.position);
	//		pathfindingState = PathfindingState.Chasing;
	//	}
	//}
	//
	//private void PreyAI(bool useOld = true)
	//{
	//	timeSinceLastBump += Time.deltaTime;
	//	bool hasBumped = false;
	//	if (timeSinceLastBump > timePerBump && Vector3.Distance(player.transform.position, transform.position) < agent.radius * 4)
	//	{
	//		hasBumped = true;
	//		timeSinceLastBump = 0f;
	//		Debug.Log("A Threat just touched me!");
	//	}
	//	// Keep fleeing if you're at the last pathfinding destination, or start fleeing if you weren't, or rethink your escape plan because you just bumped into a human!
	//	if (agent.remainingDistance <= agent.stoppingDistance || pathfindingState != PathfindingState.Fleeing || hasBumped)
	//	{
	//		// Plan A of escaping the dreadful human: Run directly away from the human!
	//		Debug.Log("Threat Detected! Initiate Escape Plan A!");
	//		Vector3 playerDirection = (player.transform.position - transform.position).normalized;
	//		Vector3 destination = transform.position - (playerDirection * (AverageSightDist() + fleeingSearchRange));
	//		if (NavMesh.SamplePosition(destination, out NavMeshHit hit, fleeingSearchRange, NavMesh.AllAreas))
	//		{
	//			agent.SetDestination(hit.position);
	//			pathfindingState = PathfindingState.Fleeing;
	//		}
	//		else
	//		{
	//			// If that's impossible to do without staying in-range, enter Plan B of escaping the dreadful human: Run sideways away from the human!
	//			Debug.Log("Plan A Failed! Initiate Escape Plan B!");
	//			Vector3 leftDirection = Quaternion.AngleAxis(90, Vector3.up) * playerDirection;
	//			Vector3 leftDestination = transform.position - (leftDirection * (AverageSightDist() + fleeingSearchRange));
	//			Vector3 rightDirection = Quaternion.AngleAxis(-90, Vector3.up) * playerDirection;
	//			Vector3 rightDestination = transform.position - (rightDirection * (AverageSightDist() + fleeingSearchRange));
	//			// Choose the most ideal direction to get far away from the human!
	//			destination = Vector3.Distance(player.transform.position, leftDestination) < Vector3.Distance(player.position, rightDestination)
	//				? leftDestination
	//				: rightDestination;
	//			if (NavMesh.SamplePosition(destination, out NavMeshHit hit1, fleeingSearchRange, NavMesh.AllAreas))
	//			{
	//				agent.SetDestination(hit1.position);
	//				pathfindingState = PathfindingState.Fleeing;
	//			}
	//			else
	//			{
	//				// If that direction cannot get far enough, try the other direction!
	//				Debug.Log("That direction didn't work! Go the other way!");
	//
	//				destination = destination == leftDestination ? rightDestination : leftDestination;
	//
	//				if (NavMesh.SamplePosition(destination, out NavMeshHit hit2, fleeingSearchRange, NavMesh.AllAreas))
	//				{
	//					agent.SetDestination(hit2.position);
	//					pathfindingState = PathfindingState.Fleeing;
	//				}
	//				else
	//				{
	//					// You're getting cornered over here! Get as close to the wall as you can!
	//					Debug.Log("Quick! Into that corner!");
	//					Vector3 crammingDestination = transform.position - playerDirection * fleeingSearchRange;
	//					if (NavMesh.SamplePosition(crammingDestination, out NavMeshHit hit3, fleeingSearchRange, NavMesh.AllAreas))
	//					{
	//						agent.SetDestination(hit3.position);
	//						pathfindingState = PathfindingState.Fleeing;
	//					}
	//					else
	//					{
	//						Debug.Log("Break through!!!");
	//						// If you're already cornered, make a run for it and break through!
	//						Vector3 finalDirection = Quaternion.AngleAxis(180 + Random.Range(-30, 31), Vector3.up) * playerDirection;
	//						Vector3 finalDestination = transform.position - (finalDirection * (AverageSightDist() + fleeingSearchRange));
	//						if (NavMesh.SamplePosition(finalDestination, out NavMeshHit hit4, fleeingSearchRange, NavMesh.AllAreas))
	//						{
	//							agent.SetDestination(hit4.position);
	//							pathfindingState = PathfindingState.Fleeing;
	//						}
	//					}
	//				}
	//			}
	//		}
	//	}
	//}

	//private void UpdateAlertAI()
	//{
	//	if (stats.special == Ability.Alert)
	//	{
	//		foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, alertDistance))
	//		{
	//			if (hitCollider.TryGetComponent(out WanderAI ai) && ai.IsAggressive)
	//				ai.IncrementAlertness(100);
	//		}
	//	}
	//
	//	switch (stats.behavior)
	//	{
	//		case AIType.Prey:
	//			PreyAI();
	//			break;
	//		case AIType.Projectile:
	//			PredatorAI(); //TEMP
	//			break;
	//		case AIType.Charge:
	//			PredatorAI(); //TEMP
	//			break;
	//		case AIType.Grapple:
	//			PredatorAI(); //TEMP
	//			break;
	//		case AIType.Slam:
	//			PredatorAI(); //TEMP
	//			break;
	//		default:
	//			break;
	//	}
	//}

	//private void UpdateWanderAI()
	//{
	//	// Non-Alerted Behavior
	//	if (agent.remainingDistance <= agent.stoppingDistance && Alertness < stats.alertnessThreshold1) //done with path
	//	{
	//		if (RandomPoint(transform.position, wanderRadius, out Vector3 point)) //pass in our centre point and radius of area
	//		{
	//			agent.SetDestination(point);
	//			pathfindingState = PathfindingState.Wandering;
	//		}
	//	}
	//	else
	//	{
	//		// Stage 1 of Alerted Behavior
	//		if ((agent.remainingDistance <= agent.stoppingDistance || pathfindingState != PathfindingState.Looking) && Alertness >= stats.alertnessThreshold1 && Alertness < stats.alertnessThreshold2)
	//		{
	//			if (RandomPoint(player.position, investigateRadius, out Vector3 point)) //pass in our centre point and radius of area
	//			{
	//				agent.SetDestination(point);
	//				pathfindingState = PathfindingState.Looking;
	//			}
	//		}
	//		else
	//		{
	//			// Stage 2 of Alerted Behavior behaviorType
	//			if ((agent.remainingDistance <= agent.stoppingDistance || (pathfindingState != PathfindingState.Investigating && pathfindingState != PathfindingState.Fleeing)) && Alertness >= stats.alertnessThreshold2 && Alertness < stats.alertnessThreshold3)
	//			{
	//				if (stats.behavior == AIType.Prey)
	//				{
	//					PreyAI();
	//				}
	//				else
	//				{
	//					agent.SetDestination(player.position);
	//					pathfindingState = PathfindingState.Investigating;
	//				}
	//			}
	//		}
	//	}
	//}

	//private void AttackPlayer()
	//{
	//	if (timeSinceLastAttack >= shootable.FurnitureSO.attackInterval)
	//	{
	//		timeSinceLastAttack = 0f;
	//		HuntingManager.Instance.DealDamageToPlayer(shootable.FurnitureSO.damage);
	//	}
	//	else
	//	{
	//		timeSinceLastAttack += Time.deltaTime;
	//	}
	//}
}