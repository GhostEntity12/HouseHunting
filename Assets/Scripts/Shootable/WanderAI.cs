using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    enum IState { Idle, Alert, Searching };
    enum SState { Stressed, CanRelax, Relaxing}

    private Shootable shootable;
    private MeshRenderer meshRenderer;
    private Canvas alertCanvas;
    private SenseSO[] senses;
    private bool xray;
    private float timeSinceLastAttack = 0f;
    private float alertness = 0; // between 0 and 100
    private SState stressState = SState.Relaxing;
    private GameObject player;
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
        player = GameObject.FindWithTag("Player");

        senses = shootable.FurnitureSO.senses;
        agent.speed = shootable.FurnitureSO.speed;
        xray = shootable.FurnitureSO.xray;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    private void OnDrawGizmos()
    {
        if (senses == null) return;

        foreach (SenseSO sense in senses)
        {
			if ((sense.senseCategory == SenseCategory.Stealth && !IsPlayerSneaking()) || (sense.senseCategory == SenseCategory.Normal && IsPlayerSneaking())) continue;

            sense.DrawGizmo(transform);
        }
    }

    private void Update()
    {
        if (shootable.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance && alertness < 50) //done with path
        {
            if (RandomPoint(transform.position, wanderRadius, out Vector3 point))
            {
                //pass in our centre point and radius of area
                agent.SetDestination(point);
            }
        }
        else
        {
            if ((agent.remainingDistance <= agent.stoppingDistance || investigate == IState.Alert) && alertness < 75)
            {
                investigate = IState.Searching;
                if (RandomPoint(player.transform.position, 3, out Vector3 point)) //pass in our centre point and radius of area
                    agent.SetDestination(point);
            }
        }

        int[] status = shootable.GetHealth(); //fetch currentHealth and maxHealth respectfully
        agent.speed = shootable.FurnitureSO.speed * (1 - Mathf.Pow(((status[1] - status[0])/status[1]),3)); //multiplies the speed proportionally to a graph of y = 1 - x^3, where x is ((maxHealth - currentHealth) / maxHealth)

        bool detected = false;

        foreach (SenseSO sense in senses)
        {
			if ((sense.senseCategory == SenseCategory.Stealth && !IsPlayerSneaking()) || (sense.senseCategory == SenseCategory.Normal && IsPlayerSneaking())) continue;

			Vector3 senseDir = Quaternion.Euler(sense.rotOffset) * transform.forward;
            Vector3 sensePos = transform.localToWorldMatrix.MultiplyPoint3x4(sense.offset);

            bool inRange = false;

            if (sense is SenseSphereSO sphereSense)
            {
                if (player != null)
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
                            detected = true;
                            if (sphereSense.senseCategory == SenseCategory.Stealth)
                            {
                                IncrementAlertness(Time.deltaTime * 50);
                            }
                            else
                            {
                                IncrementAlertness(Time.deltaTime * 100);
                            }
                    }
                }
            }
            else if (sense is SenseConeSO coneSense)
            {
                if (player != null)
                {
                    Vector3 toPosition = (player.transform.position - sensePos).normalized;
                    float dist = (player.transform.position - sensePos).magnitude;
                    float angleToPosition = Vector3.Angle(senseDir, toPosition);

                    if (angleToPosition <= coneSense.maxAngle && dist <= coneSense.range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
                    {
                            detected = true;
                            if (coneSense.senseCategory == SenseCategory.Stealth)
                            {
                                IncrementAlertness(Time.deltaTime * 50);
                            }
                            else
                            {
                                IncrementAlertness(Time.deltaTime * 100);
                            }
                    }
                }
            }
        }
        if (!detected && stressState == SState.Stressed)
        {
            stressState = SState.CanRelax;
            StartCoroutine("RelaxTimer");
        }
        if (stressState == SState.Relaxing)
        {
            alertness -= Time.deltaTime * 10;
        }
        
        alertness = Mathf.Clamp(alertness, 0, 100);
        
        if (alertness >= 75 && player != null)
        {
            alertCanvas.enabled = true;
            if (isPredator)
            {
                //meshRenderer.material.color = Color.red;

                float distance = Vector3.Distance(transform.position, player.transform.position);

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
                    agent.SetDestination(player.transform.position);
                }
            }
            else
            {
                //meshRenderer.material.color = Color.green;
                Vector3 playerDirection = player.transform.position - (transform.position);
                Vector3 destination = (transform.position) - playerDirection;
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
        }
        else if (alertness >= 50 && player != null && investigate == IState.Idle)
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

    public void IncrementAlertness(float gain)
    {
        alertness += gain;
        if (stressState != SState.Stressed)
        {
            StopCoroutine("RelaxTimer");
            stressState = SState.Stressed;
        }
    }

    private IEnumerator RelaxTimer()
    {
        yield return new WaitForSeconds(2);
        stressState = SState.Relaxing;
    }

    private void OnSoundDetect(float volume, Vector3 source)
    {
        if ((source - transform.position).magnitude <= volume)
        {
            IncrementAlertness(volume);
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