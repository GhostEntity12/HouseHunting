using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject bulletHolePrefab;
    public float lifespan;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.transform.TryGetComponent(out Hitbox hitbox))
        {
            hitbox.Damage(damage);

            // Instantiate bullet hole
            ContactPoint contact = collision.GetContact(0);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point, rotation);
            bulletHole.transform.position += bulletHole.transform.forward / 1000;
            Destroy(gameObject);
            return;
        }

        //Despawn bullet
        Destroy(gameObject, lifespan); 
    }
    
}
