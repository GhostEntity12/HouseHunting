using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletHolePrefab;

    private int damage;

    public int Damage { get { return damage; } set { damage = value; } }
    public bool CanBounce { get; set; }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Hitbox hitbox))
        {
            hitbox.Damage(damage);
        }

        Destroy(gameObject, CanBounce ? 1.5f : 0);
        // Instantiate bullet hole
        ContactPoint contact = collision.GetContact(0);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point, rotation);
        bulletHole.transform.position += bulletHole.transform.forward / 500;
        Destroy(bulletHole, 1.5f);
    }
    
}
