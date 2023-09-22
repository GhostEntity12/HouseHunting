using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Charge Behaviour", menuName = "Behaviours/Charge")]
public class ChargeBehaviour : AIBehaviour
{
    public int damage = 10; // Damage per collision
    public float chargeDelay = 2f; // Seconds per charge
    public float turnRange = 3f; // This is one of the parameters used to cancel a charge early; if the player has gone too far away from the furniture it will stop and turn.
    public float horizontalKnockbackMultiplier = 0.1f; // Determines how much the horizontal knockback is multiplied. The multiplier should be small to prevent excessive flinging.
    public float verticalKnockback = 4f; // Determines how hard the player is flung upwards. Not nearly as sensitive and varied as the horizontal one.
    [Range(0, 180)] public float maxTurnAngle = 120; // Determines the angle in which the furniture decides to stop charging when out of range. Has to be within 0 - 180 degrees since that's the limit for Vector3.Angle
    private float timeSinceLastCharge = 0;
    private bool charging = false;
    private Collider hitbox;
    private bool hitPlayer = false;

    public override void Act(ref Knowledge knowledge)
	{
        if (charging)
        {
            knowledge.Agent.isStopped = false;
            // If charging, keep trying to collide with the player until it happens or the charge is cancelled.
            if (!hitPlayer)
            {
                // Collides with player. Feels a bit dodgy since it might not perfectly reflect the furniture's total hitbox but it seemed more responsive at the time than using the furniture's own collider trigger.
                // Anyone can feel free to change this if there are any problems.
                Collider[] hitColliders = Physics.OverlapBox(hitbox.bounds.center, hitbox.bounds.size, knowledge.AITransform.rotation);

                foreach (Collider hitCollider in hitColliders)
                {
                    // If the player manages to collide with the furniture, deal damage and apply knockback! Make sure this only happens once per charge.
                    if (hitCollider.CompareTag("Player"))
                    {
                        HuntingManager.Instance.DealDamageToPlayer(damage);
                        hitPlayer = true;
                        hitCollider.transform.parent.GetComponent<Player>().ApplyKnockback(new Vector3(knowledge.Agent.velocity.x * horizontalKnockbackMultiplier, verticalKnockback, knowledge.Agent.velocity.z * horizontalKnockbackMultiplier));
                        break;
                    }
                }
            }
            // If the agent finishes pathfinding, or the player's position is out of range AND somewhere behind the furniture, stop the charge.
            Vector3 targetDir = knowledge.PlayerPosition - knowledge.AITransform.position;
            float angle = Vector3.Angle(targetDir, knowledge.AITransform.forward);
            if (knowledge.Agent.remainingDistance < 1f || (Vector3.Distance(knowledge.AITransform.position,knowledge.PlayerPosition) > turnRange && angle > 120))
            {
                charging = false;
                knowledge.Agent.SetDestination(knowledge.AITransform.position); // Somehow relying on isStopped doesn't work. This code however, DOES work!
            }
        }
        else
        {
            // Make sure the furniture is stationary and always turning to face the player, counting on a timer.
            knowledge.Agent.isStopped = true;
            Quaternion lookRotation = Quaternion.LookRotation((new Vector3(knowledge.PlayerPosition.x, knowledge.AITransform.position.y, knowledge.PlayerPosition.z) - knowledge.AITransform.position).normalized);
            knowledge.AITransform.rotation = Quaternion.Slerp(knowledge.AITransform.rotation, lookRotation, Time.deltaTime * 5);
            timeSinceLastCharge += Time.deltaTime;
            // Once time is up, let the agent move again, start the charge and reset some variables.
            if (timeSinceLastCharge >= chargeDelay)
            {
                knowledge.Agent.isStopped = false;
                charging = true;
                hitPlayer = false;
                timeSinceLastCharge = 0;
                // Get the position to charge towards. This is typically at around where the player is, except slightly further. As a fallback it will try to pathfind directly to the player instead if the navmesh search fails.
                Vector3 direction = Quaternion.AngleAxis(180, Vector3.up) * (knowledge.PlayerPosition - knowledge.AITransform.position).normalized;
                Vector3 destination = knowledge.PlayerPosition - (direction * 2f * turnRange);
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, turnRange, NavMesh.AllAreas))
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
        // Setup the collider and speed
        hitbox = knowledge.AITransform.GetComponent<Collider>();
        knowledge.Agent.speed = knowledge.Info.speed * 3;
    }
    public override void Exit(ref Knowledge knowledge) { }
}