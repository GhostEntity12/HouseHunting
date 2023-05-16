using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    enum IState { Idle, Alert, Searching };

    private Shootable shootable;
    private MeshRenderer meshRenderer;
    private Canvas alertCanvas;
    private float perceptionRadius;
    private float sneakingPerceptionRadius;
    private float height;
    private int fieldOfView;
    private float gunshotHearingRadius;
    private float walkingHearingRadius;
    private float sneakingHearingRadius;
    private bool isAlertedByGunshot = false;
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

        perceptionRadius = shootable.FurnitureSO.perceptionRadius;
        sneakingPerceptionRadius = shootable.FurnitureSO.sneakingPerceptionRadius;
        fieldOfView = shootable.FurnitureSO.fieldOfView;
        gunshotHearingRadius = shootable.FurnitureSO.gunshotHearingRadius;
        walkingHearingRadius = shootable.FurnitureSO.walkingHearingRadius;
        sneakingHearingRadius = shootable.FurnitureSO.sneakingHearingRadius;
        agent.speed = shootable.FurnitureSO.speed;
        height = shootable.FurnitureSO.height;
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((transform.position + new Vector3(0,height,0)), sneakingHearingRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((transform.position + new Vector3(0,height,0)), walkingHearingRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((transform.position + new Vector3(0,height,0)), gunshotHearingRadius);

        // Raycast currently doesn't work, so it has been commented out in the meantime.

        //int defaultLayer = 0;
        //int playerLayer = 9;
        //int defaultLayerMask = 1 << defaultLayer;
        //int playerLayerMask = 1 << playerLayer;
        //int finalMask = defaultLayerMask | playerLayerMask;

        //RaycastHit hit;

        if (subject != null)
        {
            Vector3 toPosition = (subject.transform.position - (transform.position + new Vector3(0,height,0))).normalized;
            float angleToPosition = Vector3.Angle(transform.forward, toPosition);

            if (angleToPosition <= fieldOfView ) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), toPosition);
                Gizmos.color = Color.white;
            }
            else
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), toPosition);
                Gizmos.color = Color.blue;
            }
            
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireSphere((transform.position + new Vector3(0,height,0)), perceptionRadius);

        Quaternion leftRayRotation = Quaternion.AngleAxis(-fieldOfView, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(fieldOfView, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward * perceptionRadius;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * perceptionRadius;
        Vector3 forwardDirection = Vector3.Lerp(leftRayDirection, rightRayDirection, 0.5f);

        float gapLength = (leftRayDirection - rightRayDirection).magnitude;

        Vector3 upRayDirection = forwardDirection + new Vector3(0, gapLength / 2, 0);
        Vector3 downRayDirection = forwardDirection + new Vector3(0, gapLength / -2, 0);

        Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), upRayDirection);
        Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), downRayDirection);
        Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), leftRayDirection);
        Gizmos.DrawRay((transform.position + new Vector3(0,height,0)), rightRayDirection);
        Gizmos.DrawLine((transform.position + new Vector3(0,height,0)) + downRayDirection, (transform.position + new Vector3(0,height,0)) + upRayDirection);
        Gizmos.DrawLine((transform.position + new Vector3(0,height,0)) + leftRayDirection, (transform.position + new Vector3(0,height,0)) + rightRayDirection);

    }

    private void Update()
    {
        if (shootable.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        if (IsPlayerSneaking())
        {
            perceptionRadius = shootable.FurnitureSO.perceptionRadius / 2;
            //Debug.Log("player is sneaking");
        }
        else {
            perceptionRadius = shootable.FurnitureSO.perceptionRadius;
            //Debug.Log("player is not sneaking");
        }

        if (agent.remainingDistance <= agent.stoppingDistance && alertness < 50) //done with path
        {
            if (RandomPoint(transform.position, wanderRadius, out Vector3 point)) //pass in our centre point and radius of area
                agent.SetDestination(point);
        }
        else
        {
            if (agent.remainingDistance <= agent.stoppingDistance || investigate == IState.Alert)
            {
                investigate = IState.Searching;
                if (RandomPoint(subject.transform.position, 3, out Vector3 point)) //pass in our centre point and radius of area
                    agent.SetDestination(point);
            }
        }

        int[] status = shootable.GetHealth(); //fetch currentHealth and maxHealth respectfully
        agent.speed = shootable.FurnitureSO.speed * (1 - Mathf.Pow(((status[1] - status[0])/status[1]),3)); //multiplies the speed proportionally to a graph of y = 1 - x^3, where x is ((maxHealth - currentHealth) / maxHealth)

        Collider[] hitColliders = Physics.OverlapSphere((transform.position + new Vector3(0,height,0)), perceptionRadius);

        bool detected = false;

        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 toPosition = (hitCollider.transform.position - (transform.position + new Vector3(0,height,0))).normalized;
            float angleToPosition = Vector3.Angle(transform.forward, toPosition);
            if (hitCollider.CompareTag("Player") && angleToPosition <= fieldOfView)
            {
                subject = hitCollider.gameObject;
                detected = true;
                break;
            }
        }
        if (detected)
        {
            alertness += Time.deltaTime * 100;
        }
        else
        {
            alertness -= Time.deltaTime * 10;
        }
        alertness = Mathf.Clamp(alertness, 0, 100);
        if (alertness >= 50 && subject != null && investigate == IState.Idle)
        {
            investigate = IState.Alert;
        }
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
        GameObject player = GameObject.FindWithTag("Player");
        float distance = Vector3.Distance((transform.position + new Vector3(0,height,0)), player.transform.position);
        if (distance <= gunshotHearingRadius)
        {
            alertness += 80;
            //StopAllCoroutines();
            //StartCoroutine(DoublePerceptionRadius());
        }
    }

    private IEnumerator DoublePerceptionRadius()
    {
        // if it's already doubled, don't double it again
        if (!isAlertedByGunshot)
            perceptionRadius *= 2;

        isAlertedByGunshot = true;

        yield return new WaitForSeconds(10f);

        if (isAlertedByGunshot)
            perceptionRadius /= 2;

        isAlertedByGunshot = false;
    }

    private bool IsPlayerSneaking()
    {
        if (playerMovement != null)
        {
            return playerMovement.isSneaking;
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