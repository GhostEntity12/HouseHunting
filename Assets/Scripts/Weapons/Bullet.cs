using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject bulletHole;
    public float lifespan;
    
    void OnCollisionEnter(Collision collision)
    {
        //on collision with shootable furniture, take damage
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
            Destroy(gameObject);

        }

        if (collision.gameObject.tag == "floor")
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        //DeSpawn bullet
        Destroy(gameObject, lifespan); 
    }
    
}
