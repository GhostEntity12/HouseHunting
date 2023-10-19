using System;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Slam Behaviour", menuName = "Behaviours/Slam")]
public class SlamBehaviour : AIBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private float turningSpeed = 5f;

    private float timeSinceLastJump = 0;
    private bool jumping = false;
    private float slamSearchRange = 0.5f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float distance = 1f;
    private float jumpTime = 0f;

    public override void Act(ref Knowledge knowledge)
    {
        if (jumping)
        {
            jumpTime += (Time.deltaTime / distance) * knowledge.Info.speed;
            knowledge.AITransform.position = Parabola(startPosition, endPosition, Mathf.Sqrt(distance / range) * range, jumpTime);
            if (jumpTime >= 1f)
            {
                Collider[] hitColliders = Physics.OverlapSphere(knowledge.AITransform.position, radius);
                foreach (Collider hitCollider in hitColliders)
                {
                    Player? player = hitCollider.GetComponentInParent<Player>();
                    if (player != null)
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
            knowledge.AITransform.rotation = Quaternion.Slerp(knowledge.AITransform.rotation, lookRotation, Time.deltaTime * turningSpeed);
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
        }
    }
    public override void Entry(ref Knowledge knowledge)
    {
        knowledge.Agent.isStopped = true;
    }
    public override void Exit(ref Knowledge knowledge) { }

    private static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
