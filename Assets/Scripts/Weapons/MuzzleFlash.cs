using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private void Update()
    {
        Destroy(gameObject, 0.1f); 
    }
}
