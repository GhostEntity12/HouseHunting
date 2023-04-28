using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    private Shootable shootable;
    private MeshRenderer meshRenderer;
    private Canvas alertCanvas;
    private float perceptionRadius;
    private bool isAlertedByGunshot = false;
    private float timeSinceLastAttack = 0f;

    public NavMeshAgent agent;
    public float wanderRadius = 15f; // how far the AI can wander
    public bool isPredator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shootable = GetComponent<Shootable>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();

        perceptionRadius = shootable.ShootableSO.perceptionRadius;
        agent.speed = shootable.ShootableSO.speed;
    }

    private void OnEnable() 
    {
        Gun.OnGunShootEvent += OnGunShoot;
    }

    private void OnDisable() 
    {
        Gun.OnGunShootEvent -= OnGunShoot;
    }

    void Update()
    {
        if (shootable.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(transform.position, wanderRadius, out point)) //pass in our centre point and radius of area
                agent.SetDestination(point);
        }

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                alertCanvas.enabled = true;
                if (isPredator)
                {
                    meshRenderer.material.color = Color.red;

                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distance <= 2f)
                    {
                        agent.isStopped = true;
                        AttackPlayer();
                    }
                    else
                    {
                        agent.isStopped = false;
                        agent.SetDestination(hitCollider.transform.position);
                    }
                } 
                else 
                {
                    meshRenderer.material.color = Color.green;
                    Vector3 playerDirection = hitCollider.transform.position - transform.position;
                    Vector3 destination = transform.position - playerDirection;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(destination, out hit, 2.0f, NavMesh.AllAreas))
                        agent.SetDestination(hit.position);
                }
                break;
            }
            else
            {
                alertCanvas.enabled = false;
                meshRenderer.material.color = Color.white;
                agent.isStopped = false;
            }
        }
    }

    void OnGunShoot()
    {
        StopAllCoroutines();
        StartCoroutine(DoublePerceptionRadius());
    }

    IEnumerator DoublePerceptionRadius()
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

    void AttackPlayer()
    {
        if (timeSinceLastAttack >= shootable.ShootableSO.attackInterval)
        {
            timeSinceLastAttack = 0f;
            ForestInputManager.Instance.TakeDamage(shootable.ShootableSO.damage);
        }
        else
        {
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
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