using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundAlerter : MonoBehaviour
{
	[SerializeField] Transform soundOrigin;
	[SerializeField] Vector3 soundOriginPos;
	[SerializeField] float range;
	[SerializeField] float volume;

	static List<(Vector3 loc, float ran, float time)> sounds = new();

# if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		//Debug.Log(sounds.Count);
		//if (sounds.Count == 0) return;
		//for (int i = sounds.Count - 1; i >= 0; i--)
		//{
		//	(Vector3 loc, float ran, float time) = sounds[i];
		//	Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
		//	Handles.color = new(0, 1, 0, 0.1f);
		//	Handles.DrawSolidDisc(loc, Vector3.up, ran);
		//	time -= Time.deltaTime;
		//	if (time < 0)
		//	{
		//		sounds.RemoveAt(i);
		//		continue;
		//	}
		//	sounds[i] = (loc, ran, time);
		//}
	}
#endif

	public void DebugSound() => MakeSoundImpulse(volume, soundOrigin == null ? soundOriginPos : soundOrigin.position, rangeOverride: range);

	public static void MakeSoundContinuous(float volume, Vector3 source, float? rangeOverride = null, bool hasFalloff = true)
	{
		foreach (Collider hitCollider in Physics.OverlapSphere(source, rangeOverride ?? volume))
		{
			if (hitCollider.transform.TryGetComponent(out WanderAI ai))
			{
				float distance = Vector3.Distance(source, ai.transform.position); // Get the distance between the source and furniture
				float soundFalloff = hasFalloff ?
					volume :
					rangeOverride == null
						? Mathf.Pow((distance - volume) / -volume, 2f)
						: Mathf.Pow((distance - (float)rangeOverride) / -(float)rangeOverride, 2f); 
				/*  Get the soundFalloff based on the distance. Max distance is 0, Point blank is 1. 
					Original function was: 
					((distance - max) / (min - max))^2
					It has been simplified for now, but if we want to make the minimum distance to be that of the furniture's hitbox we can do that, 
					but measures must be taken so nothing bad happens if the sound came from within that hitbox.
				*/
				ai.EnqueueSound(new(source, volume * soundFalloff * Time.deltaTime));
			}
		}
# if UNITY_EDITOR
		sounds.Add((source, rangeOverride ?? volume, Time.deltaTime));
#endif
	}
	public static void MakeSoundImpulse(float volume, Vector3 source, bool hasFalloff = true, float? rangeOverride = null)
	{

		foreach (Collider hitCollider in Physics.OverlapSphere(source, rangeOverride ?? volume))
		{
			if (hitCollider.transform.TryGetComponent(out WanderAI ai))
			{
				float distance = Vector3.Distance(source, ai.transform.position); // Get the distance between the source and furniture
				float soundFalloff = hasFalloff ?
					volume :
					rangeOverride == null
						? Mathf.Pow((distance - volume) / -volume, 2f)
						: Mathf.Pow((distance - (float)rangeOverride) / -(float)rangeOverride, 2f);
				/*  Get the soundFalloff based on the distance. Max distance is 0, Point blank is 1. 
					Original function was: 
					((distance - max) / (min - max))^2
					It has been simplified for now, but if we want to make the minimum distance to be that of the furniture's hitbox we can do that, 
					but measures must be taken so nothing bad happens if the sound came from within that hitbox.
				*/
				ai.EnqueueSound(new(source, volume * soundFalloff));
			}
		}
#if UNITY_EDITOR
		sounds.Add((source, rangeOverride ?? volume, 0.5f));
#endif
	}

}

public struct SoundAlert
{
	public Vector3 position;
	public float volume;

	public SoundAlert(Vector3 pos, float vol)
	{
		position = pos;
		volume = vol;
	}
}
