using UnityEngine;

[CreateAssetMenu(fileName = "Search Behaviour", menuName = "Behaviours/Search")]
public class SearchBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		if (knowledge.CanSeePlayer)
		{
			// Cache the player's position in case it loses sight of the player
			knowledge.dangerPosition = knowledge.PlayerPosition;
		}
		//else if (knowledge.mostProminentSound.volume > 0)
		//{
		//	knowledge.dangerPosition = knowledge.mostProminentSound.position;
		//}
		else if (knowledge.dangerPosition == null)
		{
			// Hasn't yet seen and can't see the player in this state.
			// Generate a random rotation
			Vector2 circle = Random.insideUnitCircle;
			knowledge.dangerPosition = new(
				knowledge.AITransform.position.x + circle.x,
				knowledge.AITransform.position.y,
				knowledge.AITransform.position.z + circle.y);
		}

		Quaternion targetRotation = Quaternion.LookRotation((Vector3)(knowledge.dangerPosition - knowledge.AITransform.position), Vector3.up);
		// Rotate towards dangerPosition
		knowledge.AITransform.rotation = Quaternion.RotateTowards(knowledge.AITransform.rotation, targetRotation, knowledge.Agent.angularSpeed * Time.deltaTime);
		// Check if rot is close to looking at danger position
		// If yes, reset dangerPosition
		if (1 - Mathf.Abs(Quaternion.Dot(knowledge.AITransform.rotation, targetRotation)) < 0.1f)
		{
			knowledge.dangerPosition = null;
		}
	}
}
