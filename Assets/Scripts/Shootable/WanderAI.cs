using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    enum IState { Idle, Alert, Searching };

    private Shootable shootable;
    private MeshRenderer meshRenderer;
    private Canvas alertCanvas;
    private SenseSO[] senses;
    private bool xray;
    private float timeSinceLastAttack = 0f;
    private float alertness = 0; // between 0 and 100
    private GameObject subject;
    private IState investigate = IState.Idle;
    
    public PlayerMovement playerMovement;
    public NavMeshAgent agent;
    public float wanderRadius = 15f; // how far the AI can wander
    public bool isPredator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shootable = GetComponent<Shootable>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();
        subject = GameObject.FindWithTag("Player");

        senses = shootable.FurnitureSO.senses;
        agent.speed = shootable.FurnitureSO.speed;
        xray = shootable.FurnitureSO.xray;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void OnEnable() 
    {
        Gun.OnGunShootEvent += OnGunShoot;
    }

    private void OnDisable() 
    {
        Gun.OnGunShootEvent -= OnGunShoot;
    }

    private void OnDrawGizmos()
    {
        if (senses == null)
        {
            return;
        }
        foreach (SenseSO sense in senses)
        {
            bool unused = false;
            switch (sense.senseCategory)
            {
                case SenseCategory.Quiet:
                    if (!IsPlayerSneaking())
                    {
                        unused = true;
                    }
                    break;
                case SenseCategory.Normal:
                    if (IsPlayerSneaking())
                    {
                        unused = true;
                    }
                    break;
                default:
                    break;
            }
            if (unused)
            {
                continue;
            }

            Vector3 senseDir = Quaternion.Euler(sense.rotOffset) * transform.forward;
            Vector3 sensePos = transform.localToWorldMatrix.MultiplyPoint3x4(sense.offset);

            Gizmos.color = sense.debugIdleColor; // Default Color

            if (sense is SenseSphereSO sphereSense)
            {
                if (subject != null)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(sensePos, sphereSense.radius);

                    bool detected = false;

                    foreach (Collider hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag("Player"))
                        {
                            detected = true;
                            break;
                        }
                    }

                    if (detected && (sphereSense.senseType == SenseType.Sight || (sphereSense.senseType == SenseType.Sound && IsPlayerMoving() && sphereSense.senseCategory != SenseCategory.Loud)))
                    {
                        Gizmos.color = sphereSense.debugDetectedColor;
                    }
                    Gizmos.DrawWireSphere(sensePos, sphereSense.radius);
                }
            }
            else if (sense is SenseConeSO coneSense)
            {
                if (subject != null)
                {
                    Vector3 toPosition = (subject.transform.position - sensePos).normalized;
                    float dist = (subject.transform.position - sensePos).magnitude;
                    float angleToPosition = Vector3.Angle(senseDir, toPosition);

                    if (angleToPosition <= coneSense.maxAngle && dist <= coneSense.range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay(sensePos, toPosition);
                        if (coneSense.senseType == SenseType.Sight || (coneSense.senseType == SenseType.Sound && IsPlayerMoving() && coneSense.senseCategory != SenseCategory.Loud))
                        {
                            Gizmos.color = coneSense.debugDetectedColor;
                        }
                    }
                    else
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawRay(sensePos, toPosition);
                        Gizmos.color = coneSense.debugIdleColor;
                    }
                }

                Quaternion leftRayRotation = Quaternion.AngleAxis(-coneSense.maxAngle, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(coneSense.maxAngle, Vector3.up);

                Vector3 leftRayDirection = leftRayRotation * (senseDir) * coneSense.range;
                Vector3 rightRayDirection = rightRayRotation * (senseDir) * coneSense.range;
                Vector3 forwardDirection = Vector3.Lerp(leftRayDirection, rightRayDirection, 0.5f);

                float gapLength = (leftRayDirection - rightRayDirection).magnitude;

                Vector3 upRayDirection = forwardDirection + new Vector3(0, gapLength / 2, 0);
                Vector3 downRayDirection = forwardDirection + new Vector3(0, gapLength / -2, 0);

                Gizmos.DrawRay(sensePos, upRayDirection);
                Gizmos.DrawRay(sensePos, downRayDirection);
                Gizmos.DrawRay(sensePos, leftRayDirection);
                Gizmos.DrawRay(sensePos, rightRayDirection);
                Gizmos.DrawLine(sensePos + downRayDirection, sensePos + upRayDirection);
                Gizmos.DrawLine(sensePos + leftRayDirection, sensePos + rightRayDirection);

            }
        }
    }

    private void Update()
    {
        if (shootable.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        /*
        if (IsPlayerSneaking())
        {
            Debug.Log("player is sneaking");
        }
        else {
            Debug.Log("player is not sneaking");
        }
        */

        if (agent.remainingDistance <= agent.stoppingDistance && alertness < 50) //done with path
        {
            if (RandomPoint(transform.position, wanderRadius, out Vector3 point)) //pass in our centre point and radius of area
                agent.SetDestination(point);
        }
        else
        {
            if ((agent.remainingDistance <= agent.stoppingDistance || investigate == IState.Alert) && alertness < 75)
            {
                investigate = IState.Searching;
                if (RandomPoint(subject.transform.position, 3, out Vector3 point)) //pass in our centre point and radius of area
                    agent.SetDestination(point);
            }
        }

        int[] status = shootable.GetHealth(); //fetch currentHealth and maxHealth respectfully
        agent.speed = shootable.FurnitureSO.speed * (1 - Mathf.Pow(((status[1] - status[0])/status[1]),3)); //multiplies the speed proportionally to a graph of y = 1 - x^3, where x is ((maxHealth - currentHealth) / maxHealth)

        bool detected = false;

        foreach (SenseSO sense in senses)
        {
            bool unused = false;
            switch (sense.senseCategory)
            {
                case SenseCategory.Quiet:
                    if (!IsPlayerSneaking())
                    {
                        unused = true;
                    }
                    break;
                case SenseCategory.Normal:
                    if (IsPlayerSneaking())
                    {
                        unused = true;
                    }
                    break;
                default:
                    unused = true;
                    break;
            }
            if (unused)
            {
                continue;
            }

            Vector3 senseDir = Quaternion.Euler(sense.rotOffset) * transform.forward;
            Vector3 sensePos = transform.localToWorldMatrix.MultiplyPoint3x4(sense.offset);

            bool inRange = false;

            if (sense is SenseSphereSO sphereSense)
            {
                if (subject != null)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(sensePos, sphereSense.radius);

                    foreach (Collider hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag("Player"))
                        {
                            inRange = true;
                            break;
                        }
                    }

                    if (inRange)
                    {
                        if (sphereSense.senseType == SenseType.Sight)
                        {
                            detected = true;
                            if (sphereSense.senseCategory == SenseCategory.Quiet)
                            {
                                alertness += Time.deltaTime * 50;
                            }
                            else
                            {
                                alertness += Time.deltaTime * 100;
                            }
                        }
                        else if (sphereSense.senseType == SenseType.Sound && IsPlayerMoving() && sphereSense.senseCategory != SenseCategory.Loud)
                        {
                            detected = true;
                            if (sphereSense.senseCategory == SenseCategory.Quiet)
                            {
                                alertness += Time.deltaTime * 10;
                            }
                            else
                            {
                                alertness += Time.deltaTime * 25;
                            }
                        }
                    }
                }
            }
            else if (sense is SenseConeSO coneSense)
            {
                if (subject != null)
                {
                    Vector3 toPosition = (subject.transform.position - sensePos).normalized;
                    float dist = (subject.transform.position - sensePos).magnitude;
                    float angleToPosition = Vector3.Angle(senseDir, toPosition);

                    if (angleToPosition <= coneSense.maxAngle && dist <= coneSense.range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
                    {
                        if (coneSense.senseType == SenseType.Sight)
                        {
                            detected = true;
                            if (coneSense.senseCategory == SenseCategory.Quiet)
                            {
                                alertness += Time.deltaTime * 50;
                            }
                            else
                            {
                                alertness += Time.deltaTime * 100;
                            }
                        }
                        else if (coneSense.senseType == SenseType.Sound && IsPlayerMoving() && coneSense.senseCategory != SenseCategory.Loud)
                        {
                            detected = true;
                            if (coneSense.senseCategory == SenseCategory.Quiet)
                            {
                                alertness += Time.deltaTime * 10;
                            }
                            else
                            {
                                alertness += Time.deltaTime * 25;
                            }
                        }
                    }
                }
            }
        }

        if (!detected)
        {
            alertness -= Time.deltaTime * 10;
        }
        alertness = Mathf.Clamp(alertness, 0, 100);
        
        if (alertness >= 75 && subject != null)
        {
            alertCanvas.enabled = true;
            if (isPredator)
            {
                //meshRenderer.material.color = Color.red;

                float distance = Vector3.Distance(transform.position, subject.transform.position);

                if (distance <= 2f)
                {
                    agent.isStopped = true;
                    if (alertness >= 100)
                    {
                        AttackPlayer();
                    }
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(subject.transform.position);
                }
            }
            else
            {
                //meshRenderer.material.color = Color.green;
                Vector3 playerDirection = subject.transform.position - (transform.position);
                Vector3 destination = (transform.position) - playerDirection;
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
        }
        else if (alertness >= 50 && subject != null && investigate == IState.Idle)
        {
            investigate = IState.Alert;
        }
        else
        {
            alertCanvas.enabled = false;
            //meshRenderer.material.color = Color.white;
            agent.isStopped = false;
            investigate = IState.Idle;
        }
    }

    private void OnGunShoot()
    {
        bool detected = false;

        foreach (SenseSO sense in senses)
        {
            if (sense.senseCategory != SenseCategory.Loud || sense.senseType != SenseType.Sound)
            {
                continue;
            }

            Vector3 senseDir = Quaternion.Euler(sense.rotOffset) * transform.forward;
            Vector3 sensePos = transform.localToWorldMatrix.MultiplyPoint3x4(sense.offset);

            if (sense is SenseSphereSO sphereSense)
            {
                if (subject != null)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(sensePos, sphereSense.radius);

                    foreach (Collider hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag("Player"))
                        {
                            detected = true;
                            break;
                        }
                    }
                }
                else if (sense is SenseConeSO coneSense)
                {
                    if (subject != null)
                    {
                        Vector3 toPosition = (subject.transform.position - sensePos).normalized;
                        float dist = (subject.transform.position - sensePos).magnitude;
                        float angleToPosition = Vector3.Angle(senseDir, toPosition);

                        if (angleToPosition <= coneSense.maxAngle && dist <= coneSense.range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
                        {
                            detected = true;
                            break;
                        }
                    }
                }
            }
        }
        if (detected)
        {
            alertness += 80;
        }
    }

    private bool IsPlayerSneaking()
    {
        if (playerMovement != null)
        {
            return playerMovement.isSneaking;
        }

        return false;
    }

    private bool IsPlayerMoving()
    {
        if (playerMovement != null)
        {
            return playerMovement.isMoving;
        }

        return false;
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
}