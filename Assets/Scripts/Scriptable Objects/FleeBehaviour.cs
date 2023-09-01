using UnityEngine;

[CreateAssetMenu(fileName = "Flee Behaviour", menuName = "Behaviours/Flee General")]
public class FleeBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// Update dangerPosition if required
		if (knowledge.CanSeePlayer)
		{
			knowledge.dangerPosition = knowledge.PlayerPosition;
		}
		else if (knowledge.MostProminentSound != null)
		{
			knowledge.dangerPosition = ((SoundAlert)knowledge.MostProminentSound).position;
		}

		// If we reached the end of our current path or a new sound has been detected, generate a new path
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint((Vector3)knowledge.dangerPosition, knowledge.AITransform.position), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}
}
