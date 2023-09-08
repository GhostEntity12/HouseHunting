using UnityEngine;
using UnityEngine.AI;

// This is *super* loosely based on a bunch of stuff
// but partially adapted from https://www.youtube.com/watch?v=0VV24g1SxGU
public abstract class AIBehaviour : ScriptableObject
{
	public abstract void Act(ref Knowledge knowledge);
	public abstract void Entry(ref Knowledge knowledge);
	public abstract void Exit(ref Knowledge knowledge);
}

public class Knowledge
{
	public Transform AITransform { get; private set; }
	public Vector3 PlayerPosition { get; private set; }
	public Vector3 LurePosition { get; private set; }
	public FurnitureSO Info { get; private set; }
	public NavMeshAgent Agent { get; private set; }
	public SoundAlert? MostProminentSound { get; private set; }
	public bool CanSeePlayer { get; private set; }

	public Knowledge(Transform t, Vector3 p, Vector3 l , FurnitureSO i, NavMeshAgent a, SoundAlert? sa, bool v)
	{
		AITransform = t;
		PlayerPosition = p;
		LurePosition = l;
		Info = i;
		Agent = a;
		MostProminentSound = sa;
		CanSeePlayer = v;
	}
}
