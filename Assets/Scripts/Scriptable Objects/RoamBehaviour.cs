using UnityEngine;

[CreateAssetMenu(fileName = "Roam Behaviour", menuName = "Behaviours/Roam")]
public class RoamBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// If the agent is close to it's destination, choose a new one
		if (knowledge.Agent.remainingDistance < 1)
		{
			// Get a point in front of the furniture, and a random point
			// in a circle centered on that point, then navigate to it
			// This favours the AI moving generally forward
			if (WanderAI.RandomPoint(knowledge.AITransform.position + (knowledge.AITransform.forward * 5), 3, out Vector3 newPointForward))
			{
				knowledge.Agent.SetDestination(newPointForward);
			}
			// If a point can't be found, then get a new point centered 
			// on the AI. This lets the AI choose a new random forward
			// An intemediary step could be added where it tries to reverse first
			else if (WanderAI.RandomPoint(knowledge.AITransform.position, 10, out Vector3 newPoint))
			{
				knowledge.Agent.SetDestination(newPoint);
			}
		}
	}
}
