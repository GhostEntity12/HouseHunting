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
    public NavMeshAgent agent;
    public float range = 15f; //radius of sphere
    public bool isPredator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shootable = GetComponent<Shootable>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();

        perceptionRadius = shootable.ShootableSO.perceptionRadius;
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
        if (shootable.IsDead) return;

        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(transform.position, range, out point)) //pass in our centre point and radius of area
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
                    // Player is within perception radius, do something
                    // For example, you could set the agent's destination to the player's position:
                    meshRenderer.material.color = Color.red;
                    agent.SetDestination(hitCollider.transform.position);
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