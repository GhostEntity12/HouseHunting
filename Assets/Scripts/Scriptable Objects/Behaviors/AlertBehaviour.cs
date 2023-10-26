using UnityEngine;

[CreateAssetMenu(fileName = "Alert Behaviour", menuName = "Behaviours/Alert")]
public class AlertBehaviour : AIBehaviour
{
	[SerializeField] private SoundAlertSO sound;
	public override void Act(ref Knowledge knowledge)
	{
		// If we reached the end of our current path, generate a new one
		SoundAlerter.MakeSound(sound, knowledge.AITransform.position);
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint(knowledge.PlayerPosition, knowledge.AITransform.position), 2, out Vector3 fleeDestination))
			knowledge.Agent.SetDestination(fleeDestination);
	}

	public override void Entry(ref Knowledge knowledge)
	{
		// Ensure speed is correct
		knowledge.Agent.speed = knowledge.Info.speed * 1.5f;
	}
	public override void Exit(ref Knowledge knowledge)
	{
		// Ensure speed is correct
		knowledge.Agent.speed = knowledge.Info.speed;
		knowledge.Agent.ResetPath();
	}
}
