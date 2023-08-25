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
            ai.IncrementAlertness(volume);
        }
    }
}
