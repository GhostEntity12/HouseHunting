using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] float lifespan;

    public Rigidbody Rigidbody { get; private set; }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool CanBounce { get; set; }


	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Hitbox hitbox))
        {
            hitbox.Damage(damage);
        }

        StartCoroutine(DestroyDelay());
        // Instantiate bullet hole
        ContactPoint contact = collision.GetContact(0);
        GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point, Quaternion.LookRotation(contact.normal));
        bulletHole.transform.position += bulletHole.transform.forward / 500;
        Destroy(bulletHole, 1.5f);
    }
    
    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(CanBounce ? 1.5f : 0);
        gameObject.SetActive(false);
    }
}
