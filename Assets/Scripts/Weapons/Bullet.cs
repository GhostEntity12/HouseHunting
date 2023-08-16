using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] float lifespan;

    public Rigidbody Rigidbody { get; private set; }

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

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
