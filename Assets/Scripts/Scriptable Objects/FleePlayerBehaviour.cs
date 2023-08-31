using UnityEngine;

[CreateAssetMenu(fileName = "Flee Player Behaviour", menuName = "Behaviours/Flee Player")]
public class FleePlayerBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// If we reached the end of our current path, generate a new one
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint(knowledge.PlayerPosition, knowledge.AITransform.position), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}
}
