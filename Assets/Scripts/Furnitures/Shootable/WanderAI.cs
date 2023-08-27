using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class WanderAI : MonoBehaviour
{
	enum PathfindingState { Fleeing, Chasing, Investigating, Looking, Wandering }
	enum StressState { Stressed, CanRelax, Relaxing }

	private Shootable shootable;
	private Canvas alertCanvas;
	private ViewConeSO[] viewCones;
	private bool xray;
	private StressState stressState = StressState.Relaxing;
	private PathfindingState pathfindingState = PathfindingState.Wandering;
	private AIType behaviorType = AIType.Prey;
	private AlertRate alertRate = AlertRate.Low;
	private Ability specialAbility = Ability.None;
	private float timeSinceLastAttack = 0f;
	private (float threshold1, float threshold2, float threshold3) alertnessThresholds = (50, 75, 100);
	private float timeSinceLastBump = 0f;
	private readonly float fleeingSearchRange = 3.0f;
	private readonly float investigateRadius = 2f;
	private readonly float alertDistance = 30f;
	private readonly float timePerBump = 1f; // This is used to determine if the AI pathfinding should recalculate itself, so that it can properly escape the player.
	private readonly float aiEntityDistance = 100f; // This distance will be used to determine whether or not the AI should run or not.

	public PlayerMovement playerMovement;
	public NavMeshAgent agent;
	public float wanderRadius = 15f; // how far the AI can wander

	Transform player;
	float relaxTimer = 0f;

	public float Alertness { get; private set; } = 0;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		shootable = GetComponent<Shootable>();
		alertCanvas = GetComponentInChildren<Canvas>();

		player = HuntingManager.Instance.Player;

		// Populating from furniture SO (can probably be removed and just accessed directly)
		behaviorType = shootable.FurnitureSO.behavior;
		specialAbility = shootable.FurnitureSO.special;
		alertnessThresholds = (shootable.FurnitureSO.alertnessThreshold1, shootable.FurnitureSO.alertnessThreshold2, shootable.FurnitureSO.alertnessThreshold3);
		alertRate = shootable.FurnitureSO.alertRate;
		viewCones = shootable.FurnitureSO.senses;
		agent.speed = shootable.FurnitureSO.speed;
		xray = shootable.FurnitureSO.xray;
		playerMovement = FindObjectOfType<PlayerMovement>();
	}

	private void Update()
	{
		if (shootable.IsDead)
		{
			agent.isStopped = true;
			return;
		}

		if (!player || Vector3.Distance(player.position, transform.position) > aiEntityDistance)
			return;

		if (Time.deltaTime > 0.2f)
		{
			Debug.LogWarning("Warning: DeltaTime is at:" + Time.deltaTime);
		}

		UpdateWanderAI();

		int[] status = shootable.GetHealth(); //fetch currentHealth and maxHealth respectfully
		agent.speed = shootable.FurnitureSO.speed * (1 - Mathf.Pow(((status[1] - status[0]) / status[1]), 3)); //multiplies the speed proportionally to a graph of y = 1 - x^3, where x is ((maxHealth - currentHealth) / maxHealth)
		if (pathfindingState == PathfindingState.Looking) // When looking, slow your speed.
			agent.speed /= 2;

		bool canSeePlayer = HasLineOfSight();

		if (canSeePlayer)
		{
			IncrementAlertness(Time.deltaTime, true);
		}
		UpdateAlertness(canSeePlayer);
	}

	private float GetFleeingDist()
	{
		float senseDist = 0f;
		List<float> availableSenses = new();
		foreach (ViewConeSO sense in viewCones)
		{
			availableSenses.Add(sense.range);
		}
		if (availableSenses.Count > 0)
		{
			foreach (float usableSense in availableSenses)
			{
				senseDist += usableSense;
			}
			senseDist /= availableSenses.Count;
		}
		return senseDist;
	}

	private bool HasLineOfSight()
	{
		// Treat the player's position as the center of it's body
		// TODO: alter if the player is crouching.
		Vector3 playerCenter = player.position + Vector3.up;

		// Get the distance and angle to the player
		float dist = Vector3.Distance(playerCenter, transform.position);
		Vector3 targetDirection = playerCenter - transform.position;
		float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, transform.forward)) * Mathf.Rad2Deg;

		Debug.Log(angle);

		foreach (ViewConeSO cone in viewCones)
		{
			// If the player is in range and within angle, do a raycast, check if the hit is the player
			// NOTE: This currently casts against everything in the scene. May need to add a layermask to prevent hits with transparent objects.
			if (angle < cone.maxAngle &&
				dist < cone.range &&
				Physics.Raycast(transform.position, targetDirection, out RaycastHit hit, dist) &&
				hit.transform.CompareTag("Player"))
				return true;
		}
		return false;
	}

	private void UpdateAlertness(bool seesPlayer)
	{
		switch (stressState)
		{
			case StressState.Stressed:
				// If can see the player, keep the relax timer at 2, otherwise, start reducing the relaxTimer
				if (seesPlayer)
					relaxTimer = 2;
				else
				{
					relaxTimer -= Time.deltaTime;
					if (relaxTimer <= 0)
						stressState = StressState.Relaxing;
				}
				break;
			case StressState.Relaxing:
				Alertness -= Time.deltaTime * 10;
				break;
		}
		// Clamp alertness
		Alertness = Mathf.Clamp(Alertness, 0, 100);
		alertCanvas.enabled = Alertness != 0;

		if (Alertness >= alertnessThresholds.threshold3)
		{
			UpdateAlertAI();
		}
		else
		{
			agent.isStopped = false;
		}
	}

	private void PredatorAI()
	{
		float distance = Vector3.Distance(transform.position, player.transform.position);

		if (distance <= 2f)
		{
			agent.isStopped = true;
			if (Alertness >= alertnessThresholds.threshold3)
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

	private void Flee(Vector3? position = null)
	{
		// Allows for the furniture to flee from a position - such as potentially a missed shot?
		Vector3 fleeFrom = position ?? player.position;

		Vector3 fleeTo = transform.position + (player.position - fleeFrom);
		Debug.DrawLine(fleeTo, fleeTo + Vector3.up * 10, Color.red);
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
				if (Vector3.Distance(player.transform.position, leftDestination) < Vector3.Distance(player.position, rightDestination))
				{
					destination = leftDestination;
				}
				else
				{
					destination = rightDestination;
				}
				if (NavMesh.SamplePosition(destination, out NavMeshHit hit1, fleeingSearchRange, NavMesh.AllAreas))
				{
					agent.SetDestination(hit1.position);
					pathfindingState = PathfindingState.Fleeing;
				}
				else
				{
					// If that direction cannot get far enough, try the other direction!
					Debug.Log("That direction didn't work! Go the other way!");
					if (destination == leftDestination)
					{
						destination = rightDestination;
					}
					else
					{
						destination = leftDestination;
					}
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
		if (specialAbility == Ability.Alert)
		{
			foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, alertDistance))
			{
				if (hitCollider.TryGetComponent(out WanderAI ai) && ai.IsAggressive())
				{
					ai.IncrementAlertness(100);
				}
			}
		}

		switch (behaviorType)
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
		if (agent.remainingDistance <= agent.stoppingDistance && Alertness < alertnessThresholds.threshold1) //done with path
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
			if ((agent.remainingDistance <= agent.stoppingDistance || pathfindingState != PathfindingState.Looking) && Alertness >= alertnessThresholds.threshold1 && Alertness < alertnessThresholds.threshold2)
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
				if ((agent.remainingDistance <= agent.stoppingDistance || (pathfindingState != PathfindingState.Investigating && pathfindingState != PathfindingState.Fleeing)) && Alertness >= alertnessThresholds.threshold2 && Alertness < alertnessThresholds.threshold3)
				{
					if (behaviorType == AIType.Prey)
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
			Alertness += alertRate switch
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

		if (stressState != StressState.Stressed)
		{
			StopCoroutine("RelaxTimer");
			stressState = StressState.Stressed;
		}
	}

	public bool IsAggressive()
	{
		if (behaviorType == AIType.Prey)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private IEnumerator RelaxTimer()
	{
		yield return new WaitForSeconds(2);
		stressState = StressState.Relaxing;
	}



	//private void OnDrawGizmos()
	//{
	//    if (senses == null)
	//        return;
	//    bool inRange = false;
	//    foreach (ViewConeSO sense in senses)
	//    {
	//        Vector3 senseDir = Quaternion.Euler(sense.rotOffset) * transform.forward;
	//        Vector3 sensePos = transform.localToWorldMatrix.MultiplyPoint3x4(sense.offset);

	//        Gizmos.color = sense.debugIdleColor; // Default Color

	//        if (sense is SenseSphereSO sphereSense)
	//        {
	//            if (player != null)
	//            {
	//                Collider[] hitColliders = Physics.OverlapSphere(sensePos, sphereSense.radius);

	//                bool detected = false;

	//                foreach (Collider hitCollider in hitColliders)
	//                {
	//                    if (hitCollider.CompareTag("Player"))
	//                    {
	//                        detected = true;
	//                        inRange = true;
	//                        break;
	//                    }
	//                }

	//                if (detected)
	//                    Gizmos.color = sphereSense.debugDetectedColor;

	//                Gizmos.DrawWireSphere(sensePos, sphereSense.radius);
	//            }
	//        }
	//        else if (sense is SenseConeSO coneSense)
	//        {
	//            if (player != null)
	//            {
	//                Vector3 toPosition = (player.position - sensePos).normalized;
	//                float dist = Vector3.Distance(player.position, sensePos);
	//                float angleToPosition = Vector3.Angle(senseDir, toPosition);

	//                if (angleToPosition <= coneSense.maxAngle && dist <= coneSense.range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
	//                {
	//                    inRange = true;
	//                    Gizmos.color = Color.red;
	//                    Gizmos.DrawRay(sensePos, toPosition);
	//                    Gizmos.color = coneSense.debugDetectedColor;
	//                }
	//                else
	//                {
	//                    Gizmos.color = Color.black;
	//                    Gizmos.DrawRay(sensePos, toPosition);
	//                    Gizmos.color = coneSense.debugIdleColor;
	//                }
	//            }

	//            Quaternion leftRayRotation = Quaternion.AngleAxis(-coneSense.maxAngle, Vector3.up);
	//            Quaternion rightRayRotation = Quaternion.AngleAxis(coneSense.maxAngle, Vector3.up);

	//            Vector3 leftRayDirection = leftRayRotation * (senseDir) * coneSense.range;
	//            Vector3 rightRayDirection = rightRayRotation * (senseDir) * coneSense.range;
	//            Vector3 forwardDirection = Vector3.Lerp(leftRayDirection, rightRayDirection, 0.5f);

	//            float gapLength = Vector3.Distance(leftRayDirection, rightRayDirection);

	//            Vector3 upRayDirection = forwardDirection + new Vector3(0, gapLength / 2, 0);
	//            Vector3 downRayDirection = forwardDirection + new Vector3(0, gapLength / -2, 0);

	//            Gizmos.DrawRay(sensePos, upRayDirection);
	//            Gizmos.DrawRay(sensePos, downRayDirection);
	//            Gizmos.DrawRay(sensePos, leftRayDirection);
	//            Gizmos.DrawRay(sensePos, rightRayDirection);
	//            Gizmos.DrawLine(sensePos + downRayDirection, sensePos + upRayDirection);
	//            Gizmos.DrawLine(sensePos + leftRayDirection, sensePos + rightRayDirection);

	//        }
	//    }
	//    if (player != null && inRange)
	//    {

	//        Vector3 sightDirection = (player.transform.position - transform.position).normalized;
	//        float sightRange = Vector3.Distance(player.transform.position, transform.position);
	//        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0,1,0), sightDirection, sightRange * 1.1f);
	//        if (hits.Any(x => x.collider.CompareTag("Terrain")) || hits.Any(x => x.collider.CompareTag("Obstacle")))
	//        {
	//            Gizmos.color = Color.green;
	//            Debug.Log("Obstacle in the way!");
	//            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), player.position + new Vector3(0, 1, 0));
	//        }
	//        else
	//        {
	//            Gizmos.color = Color.red;
	//            Debug.Log("I can see the human!");
	//            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), player.position + new Vector3(0, 1, 0));
	//        }
	//    }
	//}
}