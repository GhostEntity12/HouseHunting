using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Charge Behaviour", menuName = "Behaviours/Charge")]
public class ChargeBehaviour : AIBehaviour
{
    public int damage = 10;
    public float chargeDelay = 2f;
    public float chargeRange = 3f;
    private float timeSinceLastCharge = 0;
    private bool charging = false;
    private Collider hitbox;
    private bool hitPlayer = false;

    public override void Act(ref Knowledge knowledge)
	{
        if (charging)
        {
            knowledge.Agent.isStopped = false;
            if (!hitPlayer)
            {

                Collider[] hitColliders = Physics.OverlapBox(hitbox.bounds.center, hitbox.bounds.size + new Vector3(3, 3, 3), knowledge.AITransform.rotation);

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        HuntingManager.Instance.DealDamageToPlayer(damage);
                        hitPlayer = true;
                        break;
                    }
                }
            }
            if (knowledge.Agent.remainingDistance < 1)
            {
                charging = false;
            }
        }
        else
        {
            knowledge.Agent.isStopped = true;
            Quaternion lookRotation = Quaternion.LookRotation((new Vector3(knowledge.PlayerPosition.x, knowledge.AITransform.position.y, knowledge.PlayerPosition.z) - knowledge.AITransform.position).normalized);
            knowledge.AITransform.rotation = Quaternion.Slerp(knowledge.AITransform.rotation, lookRotation, Time.deltaTime * 5);
            timeSinceLastCharge += Time.deltaTime;
            if (timeSinceLastCharge >= chargeDelay)
            {
                knowledge.Agent.speed = knowledge.Info.speed * 3;
                knowledge.Agent.isStopped = false;
                charging = true;
                timeSinceLastCharge = 0;
                Vector3 direction = Quaternion.AngleAxis(180, Vector3.up) * (knowledge.PlayerPosition - knowledge.AITransform.position).normalized;
                Vector3 destination = knowledge.AITransform.position - (direction * 2f * chargeRange);
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, chargeRange, NavMesh.AllAreas))
                {
                    knowledge.Agent.SetDestination(hit.position);
                }
                else
                {
                    knowledge.Agent.SetDestination(knowledge.PlayerPosition);
                }
            }
        }
	}
    public override void Entry(ref Knowledge knowledge)
    {
        hitbox = knowledge.AITransform.GetComponent<Collider>();
    }
    public override void Exit(ref Knowledge knowledge) { }
}