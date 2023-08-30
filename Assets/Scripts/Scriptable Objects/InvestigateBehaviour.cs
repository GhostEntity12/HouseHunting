using UnityEngine;

[CreateAssetMenu(fileName = "Investigate Behaviour", menuName = "Behaviours/Investigate")]
public class InvestigateBehaviour : AIBehaviour
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
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint((Vector3)knowledge.dangerPosition, 2, out Vector3 investigatePosition))
		{
			knowledge.Agent.SetDestination(investigatePosition);
		}
	}
}