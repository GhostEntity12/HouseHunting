using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAlerter : MonoBehaviour
{
    //public delegate void OnSoundEmit(float volume, Vector3 source);
    //public static event OnSoundEmit OnSoundEmitEvent;

    public void MakeSound(float volume, Vector3 source, float? rangeOverride = null)
    {
        //OnSoundEmitEvent?.Invoke(volume, transform.position);
        HashSet<WanderAI> furnitureInRange = new HashSet<WanderAI>();
        Collider[] hitColliders;

        if (rangeOverride != null)
            hitColliders = Physics.OverlapSphere(source, (float)rangeOverride);
        else
            hitColliders = Physics.OverlapSphere(source, volume);

        foreach (Collider hitCollider in hitColliders)
        {
            WanderAI ai = hitCollider.transform.GetComponent<WanderAI>();
            if (ai != null) 
                furnitureInRange.Add(ai);
        }

        foreach (WanderAI ai in furnitureInRange)
        {
            float distance = Vector3.Distance(source, ai.transform.position); // Get the distance between the source and furniture
            float soundFalloff; // Get the soundFalloff based on the distance. Max distance is 0, Point blank is 1. 
            // Original function was: 
            // ((distance - max) / (min - max))^2
            // It has been simplified for now, but if we want to make the minimum distance to be that of the furniture's hitbox we can do that, 
            // but measures must be taken so nothing bad happens if the sound came from within that hitbox.
            if (rangeOverride != null)
                soundFalloff = Mathf.Pow((distance - (float)rangeOverride) / -(float)rangeOverride,2f);
            else
                soundFalloff = Mathf.Pow((distance - volume) / -volume, 2f);
            ai.IncrementAlertness(volume * soundFalloff);
        }
    }
}
