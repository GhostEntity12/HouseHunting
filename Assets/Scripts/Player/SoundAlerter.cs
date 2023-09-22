using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundAlerter : MonoBehaviour
{
	[SerializeField] Transform soundOrigin;
	[SerializeField] Vector3 soundOriginPos;
	[SerializeField] float range;
	[SerializeField] float volume;

#if UNITY_EDITOR
	static List<(Vector3 loc, float ran, float time)> sounds = new();

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

	public static void MakeSound(SoundAlertSO sound, Vector3 source)
	{
		foreach (Collider hitCollider in Physics.OverlapSphere(source, sound.range))
		{
			if (hitCollider.transform.TryGetComponent(out WanderAI ai))
			{
				float volume = sound.falloff ?
					Mathf.Pow(((float)Vector3.Distance(source, ai.transform.position) - sound.range) / -sound.range, 2f) :
					sound.volume;

				/*  Get the soundFalloff based on the distance. Max distance is 0, Point blank is 1. 
					Original function was: 
					((distance - max) / (min - max))^2
					It has been simplified for now, but if we want to make the minimum distance to be that of the furniture's hitbox we can do that, 
					but measures must be taken so nothing bad happens if the sound came from within that hitbox.
				*/
				ai.EnqueueSound(new(source, sound.IsContinuous ? volume * Time.deltaTime : volume));
			}
		}
#if UNITY_EDITOR
		sounds.Add((source, sound.range, sound.IsContinuous ? Time.deltaTime : 0.5f));
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
