using UnityEngine;

[CreateAssetMenu(fileName = "Search Behaviour", menuName = "Behaviours/Search")]
public class SearchBehaviour : AIBehaviour
{
	[SerializeField] private float holdLookLength = 1;

	private Vector3? dangerPosition = null;
	private float holdLookTimer = 0;

	public override void Act(ref Knowledge knowledge)
	{
		if (knowledge.CanSeePlayer)
		{
			// Cache the player's position in case it loses sight of the player
			dangerPosition = knowledge.PlayerPosition;
			holdLookTimer = holdLookLength;
		}
		//else if (knowledge.mostProminentSound.volume > 0)
		//{
		//	knowledge.dangerPosition = knowledge.mostProminentSound.position;
		//	holdLookTimer = holdLookLength;
		//}
		else if (dangerPosition == null)
		{
			// Hasn't yet seen and can't see the player in this state.
			// Generate a random rotation
			Vector2 circle = Random.insideUnitCircle;
			dangerPosition = new(
				knowledge.AITransform.position.x + circle.x,
				knowledge.AITransform.position.y,
				knowledge.AITransform.position.z + circle.y);
			holdLookTimer = holdLookLength + Random.value - 0.5f;
		}
		Quaternion targetRotation = Quaternion.LookRotation((Vector3)(dangerPosition - knowledge.AITransform.position), Vector3.up);
		// Rotate towards dangerPosition
		knowledge.AITransform.rotation = Quaternion.RotateTowards(knowledge.AITransform.rotation, targetRotation, knowledge.Agent.angularSpeed * Time.deltaTime);
		// Check if rot is close to looking at danger position
		// If yes, reset dangerPosition
		if (1 - Mathf.Abs(Quaternion.Dot(knowledge.AITransform.rotation, targetRotation)) < 0.1f)
		{
			holdLookTimer -= Time.deltaTime;
			if (holdLookTimer <= 0)
			{
				dangerPosition = null;
				holdLookTimer = holdLookLength;
			}
		}
	}

	public override void Entry(ref Knowledge knowledge)
	{
		// Seize control of rotation
		knowledge.Agent.updateRotation = false;
	}

	public override void Exit(ref Knowledge knowledge)
	{
		knowledge.Agent.ResetPath();
		knowledge.Agent.updateRotation = true;
	}
}
