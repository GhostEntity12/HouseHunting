using UnityEngine;

public class BulletImpactSound : MonoBehaviour
{
    [SerializeField] private AudioClip impactSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, collision.contacts[0].point);
        }

        // Destroy the bullet after impact
        Destroy(gameObject);
    }
}
