using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletHolePrefab;

    private int damage;

    public int Damage { get { return damage; } set { damage = value; } }
    public bool CanBounce { get; set; }

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
