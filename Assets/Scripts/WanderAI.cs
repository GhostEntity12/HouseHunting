using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    public NavMeshAgent agent;
    public float range = 15f; //radius of sphere
	public float perceptionRadius = 10f;
    private Shootable shootable;
    [SerializeField] private Material predatorMaterial;
    [SerializeField] private Material preyMaterial;
    public bool isPredator;
    private MeshRenderer meshRenderer;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootable = GetComponent<Shootable>();
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        
        if (shootable.IsDead) return;

        if(agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(transform.position, range, out point)) //pass in our centre point and radius of area
            {
                agent.SetDestination(point);
            }
        }
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, perceptionRadius);
foreach (Collider hitCollider in hitColliders)
{
    if (hitCollider.CompareTag("Player"))
    {
		Debug.Log("Player in range!");
		if (isPredator)
		{
        // Player is within perception radius, do something
        Debug.Log("Attack!");
        // For example, you could set the agent's destination to the player's position:
        meshRenderer.material = predatorMaterial;
        agent.SetDestination(hitCollider.transform.position);
		} else {
			Debug.Log("Run!");
            meshRenderer.material = preyMaterial;
			Vector3 playerDirection = hitCollider.transform.position - transform.position;
            Vector3 destination = transform.position - playerDirection;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
		}
	}
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