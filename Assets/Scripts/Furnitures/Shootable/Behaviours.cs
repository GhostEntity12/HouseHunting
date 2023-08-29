using UnityEngine;
using UnityEngine.AI;

public abstract class Behaviour : ScriptableObject
{
	public abstract void Act(ref Knowledge knowledge);
}

public class Knowledge
{
	public Vector3? dangerPosition;

	public Vector3 AIPosition { get; private set; }
	public Vector3 AIForward { get; private set; }
	public Vector3 PlayerPosition { get; private set; }
	public FurnitureSO Stats { get; private set; }
	public NavMeshAgent Agent { get; private set; }
	public SoundAlert MostProminentSound { get; private set; }
	public bool CanSeePlayer { get; private set; }

	public Knowledge(Vector3 t, Vector3 f, Vector3 p, Vector3? d, FurnitureSO s, NavMeshAgent a, SoundAlert sa, bool v)
	{
		AIPosition = t;
		AIForward = f;
		PlayerPosition = p;
		dangerPosition = d;
		Stats = s;
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
			if (WanderAI.RandomPoint(knowledge.AIPosition + knowledge.AIForward * 5, 3, out Vector3 newPointForward))
			{
				knowledge.Agent.SetDestination(newPointForward);
			}
			// If a point can't be found, then get a new point centered 
			// on the AI. This lets the AI choose a new random forward
			// An intemediary step could be added where it tries to reverse first
			else if (WanderAI.RandomPoint(knowledge.AIPosition, 10, out Vector3 newPoint))
			{
				knowledge.Agent.SetDestination(newPoint);
			}
		}
	}
}

[CreateAssetMenu(fileName = "Search Behaviour", menuName = "Behaviours/Search")]
public class SearchBehaviour : Behaviour
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
			// Look around randomly
		}

		// Look at dangerPosition
	}
}

[CreateAssetMenu(fileName = "Flee Behaviour", menuName = "Behaviours/Flee General")]
public class FleeBehaviour : Behaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		// Update dangerPosition if required
		if (knowledge.CanSeePlayer)
		{
			knowledge.dangerPosition = knowledge.PlayerPosition;
		}
		else if (knowledge.MostProminentSound.volume > 0)
		{
			knowledge.dangerPosition = knowledge.MostProminentSound.position;
		}

		// If we reached the end of our current path or a new sound has been detected, generate a new path
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint((Vector3)knowledge.dangerPosition, knowledge.AIPosition), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}
}

[CreateAssetMenu(fileName = "Flee Player Behaviour", menuName = "Behaviours/Flee Player")]
public class FleePlayerBehaviour : Behaviour
{
	public override void Act(ref Knowledge knowledge)
	{
		if (knowledge.Agent.remainingDistance < 1 && WanderAI.RandomPoint(WanderAI.FindFleePoint(knowledge.PlayerPosition, knowledge.AIPosition), 2, out Vector3 fleeDestination))
		{
			knowledge.Agent.SetDestination(fleeDestination);
		}
	}
}
