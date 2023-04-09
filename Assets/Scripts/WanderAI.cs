using UnityEngine;
using UnityEngine.AI;

public class WanderAI : MonoBehaviour 
{
    public NavMeshAgent agent;
    public float range = 15f; //radius of sphere

    private Shootable shootable;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootable = GetComponent<Shootable>();
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