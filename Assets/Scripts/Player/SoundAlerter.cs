using UnityEngine;

public class SoundAlerter : MonoBehaviour
{
    //public delegate void OnSoundEmit(float volume, Vector3 source);
    //public static event OnSoundEmit OnSoundEmitEvent;

    public void MakeSound(float volume, Vector3 source, float? rangeOverride = null)
    {
        Collider[] hitColliders = Physics.OverlapSphere(source, rangeOverride ?? volume);

		foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out WanderAI ai))
            {
				float distance = Vector3.Distance(source, ai.transform.position); // Get the distance between the source and furniture
				float soundFalloff = rangeOverride == null
					? Mathf.Pow((distance - volume) / -volume, 2f)
					: Mathf.Pow((distance - (float)rangeOverride) / -(float)rangeOverride, 2f); /* Get the soundFalloff based on the distance. Max distance is 0, Point blank is 1. 
               Original function was: 
               ((distance - max) / (min - max))^2
               It has been simplified for now, but if we want to make the minimum distance to be that of the furniture's hitbox we can do that, 
               but measures must be taken so nothing bad happens if the sound came from within that hitbox.
            */
				ai.IncrementAlertness(volume * soundFalloff);
			}
        }
    }
}
