using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject bulletHole;
    public float lifespan;

    void Update()
    {
        //DeSpawn bullet
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DisableBullet(lifespan));
        }
    }
    




    void OnCollisionEnter(Collision collision)
    {
        // on collision with shootable furniture, take damage
        Shootable shootableTarget = collision.transform.GetComponentInParent<Shootable>();
        if (shootableTarget != null)
        {
            shootableTarget.TakeDamage(damage, collision.collider);
            //bullet hole
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;
            GameObject obj = Instantiate(bulletHole, contact.point, rotation);
            obj.transform.position += obj.transform.forward/1000;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "floor")
        {
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator DisableBullet(float lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        gameObject.SetActive(false);
    }
    
}
