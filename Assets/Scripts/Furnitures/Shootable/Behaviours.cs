using UnityEngine;
using UnityEngine.AI;

// This is *super* loosely based on a bunch of stuff
// but partially adapted from https://www.youtube.com/watch?v=0VV24g1SxGU
public abstract class Behaviour : ScriptableObject
{
	public abstract void Act(ref Knowledge knowledge);
	public abstract void Entry(ref Knowledge knowledge);
	public abstract void Exit(ref Knowledge knowledge);
}

public class Knowledge
{
	public Transform AITransform { get; private set; }
	public Vector3 PlayerPosition { get; private set; }
	public FurnitureSO Info { get; private set; }
	public NavMeshAgent Agent { get; private set; }
	public SoundAlert? MostProminentSound { get; private set; }
	public bool CanSeePlayer { get; private set; }

	public Knowledge(Transform t, Vector3 p, FurnitureSO i, NavMeshAgent a, SoundAlert? sa, bool v)
	{
		AITransform = t;
		PlayerPosition = p;
		Info = i;
		Agent = a;
		MostProminentSound = sa;
		CanSeePlayer = v;
	}
}

[CreateAssetMenu(fileName = "Roam Behaviour", menuName = "Behaviours/Roam")]
public class RoamBehaviour : Behaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// If the agent is close to it's destination, choose a new one
		if (knowledge.Agent.remainingDistance < 1)
		{
			// Get a point in front of the furniture, and a random point
			// in a circle centered on that point, then navigate to it
			// This favours the AI moving generally forward
			// If a point can't be found, then get a new point centered 
			// on the AI. This lets the AI choose a new random forward
			// An intemediary step could be added where it tries to reverse first
			if (WanderAI.RandomPoint(knowledge.AITransform.position + (knowledge.AITransform.forward * 5), 3, out Vector3 destination) ||
				WanderAI.RandomPoint(knowledge.AITransform.position, 10, out destination))
			{
				knowledge.Agent.SetDestination(destination);
			}
		}
	}

	public override void Entry(ref Knowledge knowledge)
	{

	}

	public override void Exit(ref Knowledge knowledge)
	{
		knowledge.Agent.ResetPath();
	}
}

[CreateAssetMenu(fileName = "Search Behaviour", menuName = "Behaviours/Search")]
public class SearchBehaviour : Behaviour
{
	private Vector3? dangerPosition = null;
	private float holdLookTimer;
	[SerializeField] private float holdLookLength = 1;
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
			holdLookTimer = holdLookLength;
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

[CreateAssetMenu(fileName = "Flee Behaviour", menuName = "Behaviours/Flee General")]
public class FleeBehaviour : Behaviour
{
	Vector3? dangerPosition = null;
	public override void Act(ref Knowledge knowledge)
	{
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
		if (knowledge.Agent.remainingDistance < 1 &&
			WanderAI.RandomPoint(WanderAI.FindFleePoint((Vector3)dangerPosition, knowledge.AITransform.position), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}

	public override void Entry(ref Knowledge knowledge)
	{

	}

	public override void Exit(ref Knowledge knowledge)
	{
		knowledge.Agent.ResetPath();
	}
}

[CreateAssetMenu(fileName = "Flee Player Behaviour", menuName = "Behaviours/Flee Player")]
public class FleePlayerBehaviour : Behaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// If we reached the end of our current path, generate a new one
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint(knowledge.PlayerPosition, knowledge.AITransform.position), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}

	public override void Entry(ref Knowledge knowledge)
	{
		// Ensure speed is correct
		knowledge.Agent.speed = knowledge.Info.speed * 2;
	}

	public override void Exit(ref Knowledge knowledge)
	{

		// Ensure speed is correct
		knowledge.Agent.speed = knowledge.Info.speed;
		knowledge.Agent.ResetPath();
	}
}
