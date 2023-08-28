using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class WanderAI : MonoBehaviour
{
	const float aiEntityDistance = 100f; // This distance will be used to determine whether or not the AI should run or not.
	enum PathfindingState { Fleeing, Chasing, Investigating, Looking, Wandering }
	enum AlertLevels { None, Level1, Level2, Level3}

	private Shootable shootable;
	private Canvas alertCanvas;
	private PathfindingState pathfindingState = PathfindingState.Wandering;
	private AlertLevels currentBehaviour = AlertLevels.None;
	private float timeSinceLastAttack = 0f;
	private float timeSinceLastBump = 0f;
	private bool isRelaxed;
	private float relaxTimer = 0f;

	private readonly float fleeingSearchRange = 3.0f;
	private readonly float investigateRadius = 2f;
	private readonly float alertDistance = 30f;
	private readonly float timePerBump = 1f; // This is used to determine if the AI pathfinding should recalculate itself, so that it can properly escape the player.

	private NavMeshAgent agent;
	public float wanderRadius = 15f; // how far the AI can wander

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

		//if (Time.deltaTime > 0.2f)
		//{
		//	Debug.LogWarning("Warning: DeltaTime is at:" + Time.deltaTime);
		//}
		//SetSpeed();

		//UpdateWanderAI();

		bool canSeePlayer = HasLineOfSight();

		if (canSeePlayer)
		{
			IncrementAlertness(Time.deltaTime * 10, true);
		}



		switch (currentBehaviour)
		{
			// Does not know player is around at all
			case AlertLevels.None:
				Roam();
				break;
			// Has some knowledge of the player
			case AlertLevels.Level1:
				Flee();
				break;
			default:
				break;
		}

		UpdateBehaviourFromThreshold();

		//bool canSeePlayer = HasLineOfSight();

		//if (canSeePlayer)
		//{
		//	IncrementAlertness(Time.deltaTime, true);
		//}

		//UpdateRelax(canSeePlayer);
		//alertCanvas.enabled = Alertness != 0;

		//if (Alertness >= stats.alertnessThreshold3)
		//{
		//	UpdateAlertAI();
		//}
		//else
		//{
		//	agent.isStopped = false;
		//}
	}

	private void SetSpeed()
	{
		// Multiplies the speed proportionally to a graph of y = 1 - x^3, where x is ((maxHealth - currentHealth) / maxHealth)
		(int current, int max) = shootable.Health;
		agent.speed = shootable.FurnitureSO.speed * (1 - Mathf.Pow((max - current) / max, 3));

		// TODO: what's happening here? Why?
		if (pathfindingState == PathfindingState.Looking) // When looking, slow your speed.
			agent.speed /= 2;
	}

	/// <summary>
	/// Gets the average sense distance
	/// </summary>
	/// <returns></returns>
	private float GetFleeingDist() => stats.senses.Average(vc => vc.length);

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
	/// Updates the furniture's alertness based on whether it can see the player
	/// </summary>
	/// <param name="seesPlayer"></param>
	private void UpdateRelax(bool seesPlayer)
	{
		if (isRelaxed)
		{
			Alertness -= Time.deltaTime * 10;
		}
		else
		{
			// If can see the player, keep the relax timer at max, otherwise, start reducing the relaxTimer
			if (seesPlayer)
				relaxTimer = stats.timeBeforeRelaxing;
			else
			{
				relaxTimer -= Time.deltaTime / stats.timeToRelax;
				if (relaxTimer <= 0)
					isRelaxed = true;
			}
		}
	}

	private void UpdateBehaviourFromThreshold()
	{
		switch (Alertness)
		{
			case float a3 when a3 >= stats.alertnessThreshold3 && currentBehaviour != AlertLevels.Level3:
			case float a2 when a2 >= stats.alertnessThreshold2 && currentBehaviour != AlertLevels.Level2:
			case float a1 when a1 >= stats.alertnessThreshold1 && currentBehaviour != AlertLevels.Level1:
				currentBehaviour = AlertLevels.Level1;
				agent.ResetPath();
				break;
			case float a0 when a0 == 0:
				currentBehaviour = AlertLevels.None;
				break;
			default:
				break;
		}
	}

	private void Flee(Vector3? position = null)
	{
		Debug.Log("Fleeing!");
		// Allows for the furniture to flee from a position - such as potentially a missed shot?
		Vector3 fleeFrom = position ?? player.position;

		Vector3 fleeTo = transform.position + ((transform.position - fleeFrom).normalized * GetFleeingDist());
		Debug.DrawLine(fleeFrom, transform.position, Color.cyan, 15);
		Debug.DrawLine(transform.position, fleeTo, Color.blue, 15);
		Debug.DrawLine(fleeTo, fleeTo + Vector3.up * 10, Color.red, 15);
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

	// TODO: Rework this
	private void PredatorAI()
	{
		float distance = Vector3.Distance(transform.position, player.transform.position);

		if (distance <= 2f)
		{
			agent.isStopped = true;
			if (Alertness >= stats.alertnessThreshold3)
			{
				AttackPlayer();
			}
		}
		else
		{
			agent.isStopped = false;
			agent.SetDestination(player.transform.position);
			pathfindingState = PathfindingState.Chasing;
		}
	}

	private void PreyAI(bool useOld = true)
	{
		timeSinceLastBump += Time.deltaTime;
		bool hasBumped = false;
		if (timeSinceLastBump > timePerBump && Vector3.Distance(player.transform.position, transform.position) < agent.radius * 4)
		{
			hasBumped = true;
			timeSinceLastBump = 0f;
			Debug.Log("A Threat just touched me!");
		}
		// Keep fleeing if you're at the last pathfinding destination, or start fleeing if you weren't, or rethink your escape plan because you just bumped into a human!
		if (agent.remainingDistance <= agent.stoppingDistance || pathfindingState != PathfindingState.Fleeing || hasBumped)
		{
			// Plan A of escaping the dreadful human: Run directly away from the human!
			Debug.Log("Threat Detected! Initiate Escape Plan A!");
			Vector3 playerDirection = (player.transform.position - transform.position).normalized;
			Vector3 destination = transform.position - (playerDirection * (GetFleeingDist() + fleeingSearchRange));
			if (NavMesh.SamplePosition(destination, out NavMeshHit hit, fleeingSearchRange, NavMesh.AllAreas))
			{
				agent.SetDestination(hit.position);
				pathfindingState = PathfindingState.Fleeing;
			}
			else
			{
				// If that's impossible to do without staying in-range, enter Plan B of escaping the dreadful human: Run sideways away from the human!
				Debug.Log("Plan A Failed! Initiate Escape Plan B!");
				Vector3 leftDirection = Quaternion.AngleAxis(90, Vector3.up) * playerDirection;
				Vector3 leftDestination = transform.position - (leftDirection * (GetFleeingDist() + fleeingSearchRange));
				Vector3 rightDirection = Quaternion.AngleAxis(-90, Vector3.up) * playerDirection;
				Vector3 rightDestination = transform.position - (rightDirection * (GetFleeingDist() + fleeingSearchRange));
				// Choose the most ideal direction to get far away from the human!
				destination = Vector3.Distance(player.transform.position, leftDestination) < Vector3.Distance(player.position, rightDestination)
					? leftDestination
					: rightDestination;
				if (NavMesh.SamplePosition(destination, out NavMeshHit hit1, fleeingSearchRange, NavMesh.AllAreas))
				{
					agent.SetDestination(hit1.position);
					pathfindingState = PathfindingState.Fleeing;
				}
				else
				{
					// If that direction cannot get far enough, try the other direction!
					Debug.Log("That direction didn't work! Go the other way!");

					destination = destination == leftDestination ? rightDestination : leftDestination;

					if (NavMesh.SamplePosition(destination, out NavMeshHit hit2, fleeingSearchRange, NavMesh.AllAreas))
					{
						agent.SetDestination(hit2.position);
						pathfindingState = PathfindingState.Fleeing;
					}
					else
					{
						// You're getting cornered over here! Get as close to the wall as you can!
						Debug.Log("Quick! Into that corner!");
						Vector3 crammingDestination = transform.position - playerDirection * fleeingSearchRange;
						if (NavMesh.SamplePosition(crammingDestination, out NavMeshHit hit3, fleeingSearchRange, NavMesh.AllAreas))
						{
							agent.SetDestination(hit3.position);
							pathfindingState = PathfindingState.Fleeing;
						}
						else
						{
							Debug.Log("Break through!!!");
							// If you're already cornered, make a run for it and break through!
							Vector3 finalDirection = Quaternion.AngleAxis(180 + Random.Range(-30, 31), Vector3.up) * playerDirection;
							Vector3 finalDestination = transform.position - (finalDirection * (GetFleeingDist() + fleeingSearchRange));
							if (NavMesh.SamplePosition(finalDestination, out NavMeshHit hit4, fleeingSearchRange, NavMesh.AllAreas))
							{
								agent.SetDestination(hit4.position);
								pathfindingState = PathfindingState.Fleeing;
							}
						}
					}
				}
			}
		}
	}

	private void UpdateAlertAI()
	{
		if (stats.special == Ability.Alert)
		{
			foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, alertDistance))
			{
				if (hitCollider.TryGetComponent(out WanderAI ai) && ai.IsAggressive)
					ai.IncrementAlertness(100);
			}
		}

		switch (stats.behavior)
		{
			case AIType.Prey:
				PreyAI();
				break;
			case AIType.Projectile:
				PredatorAI(); //TEMP
				break;
			case AIType.Charge:
				PredatorAI(); //TEMP
				break;
			case AIType.Grapple:
				PredatorAI(); //TEMP
				break;
			case AIType.Slam:
				PredatorAI(); //TEMP
				break;
			default:
				break;
		}
	}

	private void UpdateWanderAI()
	{
		// Non-Alerted Behavior
		if (agent.remainingDistance <= agent.stoppingDistance && Alertness < stats.alertnessThreshold1) //done with path
		{
			if (RandomPoint(transform.position, wanderRadius, out Vector3 point)) //pass in our centre point and radius of area
			{
				agent.SetDestination(point);
				pathfindingState = PathfindingState.Wandering;
			}
		}
		else
		{
			// Stage 1 of Alerted Behavior
			if ((agent.remainingDistance <= agent.stoppingDistance || pathfindingState != PathfindingState.Looking) && Alertness >= stats.alertnessThreshold1 && Alertness < stats.alertnessThreshold2)
			{
				if (RandomPoint(player.position, investigateRadius, out Vector3 point)) //pass in our centre point and radius of area
				{
					agent.SetDestination(point);
					pathfindingState = PathfindingState.Looking;
				}
			}
			else
			{
				// Stage 2 of Alerted Behavior behaviorType
				if ((agent.remainingDistance <= agent.stoppingDistance || (pathfindingState != PathfindingState.Investigating && pathfindingState != PathfindingState.Fleeing)) && Alertness >= stats.alertnessThreshold2 && Alertness < stats.alertnessThreshold3)
				{
					if (stats.behavior == AIType.Prey)
					{
						PreyAI();
					}
					else
					{
						agent.SetDestination(player.position);
						pathfindingState = PathfindingState.Investigating;
					}
				}
			}
		}
	}

	private void AttackPlayer()
	{
		if (timeSinceLastAttack >= shootable.FurnitureSO.attackInterval)
		{
			timeSinceLastAttack = 0f;
			HuntingManager.Instance.DealDamageToPlayer(shootable.FurnitureSO.damage);
		}
		else
		{
			timeSinceLastAttack += Time.deltaTime;
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

	public void IncrementAlertness(float gain, bool isRate = false)
	{
		if (isRate)
		{
			Alertness += stats.alertRate switch
			{
				AlertRate.Low => gain * 5,
				AlertRate.Medium => gain * 10,
				AlertRate.High => gain * 20,
				AlertRate.Instant => 100,
				_ => gain,
			};
		}

		// Clamp alertness to the range 0-100
		Alertness = Mathf.Clamp(Alertness, 0, 100);

		if (isRelaxed)
		{
			isRelaxed = false;
			relaxTimer = stats.timeBeforeRelaxing;
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
}