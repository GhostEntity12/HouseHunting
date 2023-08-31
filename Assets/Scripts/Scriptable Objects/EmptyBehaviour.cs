using UnityEngine;

[CreateAssetMenu(fileName = "Empty Behaviour", menuName = "Behaviours/Empty")]
public class EmptyBehaviour : AIBehaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		//Debug.Log("Using EmptyBehaviour");
	}
}
