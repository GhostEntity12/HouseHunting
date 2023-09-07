using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Slam Behaviour", menuName = "Behaviours/Slam")]
public class SlamBehaviour : AIBehaviour
{
    public float range = 10f;
    public float radius = 2f;
    public int damage = 1;
    public float cooldown = 5f;
    private float timeSinceLastJump = 0;
    private bool jumping = false;
    private float slamSearchRange = 0.5f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float distance = 1f;
    private float jumpTime = 0f;
    public override void Act(ref Knowledge knowledge)
    {
        knowledge.Agent.isStopped = true;
        if (jumping)
        {
            jumpTime += (Time.deltaTime / distance) * knowledge.Stats.speed;
            knowledge.Agent.isStopped = true;
            knowledge.AITransform.position = WanderAI.Parabola(startPosition, endPosition, range, jumpTime);
            if (jumpTime >= 1f)
            {
                foreach (Collider hitCollider in Physics.OverlapSphere(knowledge.AITransform.position, radius))
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        HuntingManager.Instance.DealDamageToPlayer(damage);
                        break;
                    }
                }
                jumping = false;
            }
        }
        else
        {
            Quaternion lookRotation = Quaternion.LookRotation((new Vector3(knowledge.PlayerPosition.x, knowledge.AITransform.position.y, knowledge.PlayerPosition.z) - knowledge.AITransform.position).normalized);
            knowledge.AITransform.rotation = Quaternion.Slerp(knowledge.AITransform.rotation, lookRotation, Time.deltaTime * 5);
            timeSinceLastJump += Time.deltaTime;
            if (timeSinceLastJump >= cooldown)
            {
                if (Vector3.Distance(knowledge.PlayerPosition, knowledge.AITransform.position) > range)
                {
                    knowledge.Agent.isStopped = false;
                    knowledge.Agent.SetDestination(knowledge.PlayerPosition);
                }
                else
                {
                    knowledge.Agent.isStopped = true;
                    if (NavMesh.SamplePosition(knowledge.PlayerPosition, out NavMeshHit hit, slamSearchRange, NavMesh.AllAreas))
                    {
                        jumping = true;
                        jumpTime = 0f;
                        timeSinceLastJump = 0f;
                        startPosition = knowledge.AITransform.position;
                        endPosition = hit.position;
                        distance = Vector3.Distance(startPosition, endPosition);
                    }
                }
            }
            else
            {
                knowledge.Agent.isStopped = true;
            }
        }
    }
}
