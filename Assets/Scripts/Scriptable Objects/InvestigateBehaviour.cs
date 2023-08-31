using UnityEngine;

[CreateAssetMenu(fileName = "Investigate Behaviour", menuName = "Behaviours/Investigate")]
public class InvestigateBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		Vector3? dangerPosition = null;
		// Update dangerPosition if required
		if (knowledge.CanSeePlayer)
		{
			dangerPosition = knowledge.PlayerPosition;
		}
		else if (knowledge.MostProminentSound != null)
		{
			dangerPosition = ((SoundAlert)knowledge.MostProminentSound).position;
		}
		// If we reached the end of our current path or a new sound has been detected, generate a new path
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint((Vector3)dangerPosition, 2, out Vector3 investigatePosition))
		{
			knowledge.Agent.SetDestination(investigatePosition);
		}
	}

	public override void Entry(ref Knowledge knowledge) { }

	public override void Exit(ref Knowledge knowledge) { }
}