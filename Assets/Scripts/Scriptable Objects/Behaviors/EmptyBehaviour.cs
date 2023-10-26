using UnityEngine;

[CreateAssetMenu(fileName = "Empty Behaviour", menuName = "Behaviours/Empty")]
public class EmptyBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge) { }

	public override void Entry(ref Knowledge knowledge) { }

	public override void Exit(ref Knowledge knowledge) { }
}
