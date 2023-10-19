using UnityEngine;

[CreateAssetMenu(fileName = "Panic Behaviour", menuName = "Behaviours/Panic")]
public class PanicBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// If the agent is close to it's destination, choose a new one
		if (knowledge.Agent.remainingDistance < 1)
		{
			// Get a random point centered on the AI with a random range.
			// Nothing more here, it's as simple as 
			if (WanderAI.RandomPoint(knowledge.AITransform.position, Random.Range(3,5), out Vector3 destination))
				knowledge.Agent.SetDestination(destination);
		}
	}

	public override void Entry(ref Knowledge knowledge)
	{
		// Ensure speed is correct
		knowledge.Agent.speed = knowledge.Info.speed * 2f;
	}

	public override void Exit(ref Knowledge knowledge)
	{
		knowledge.Agent.ResetPath();
	}
}
