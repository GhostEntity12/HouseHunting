using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Singleton<BulletPool>
{
    // TODO: make this not a singleton? could work better in the ammo pouch
    [SerializeField] private int amountToPool;

    private readonly List<Bullet> bullets = new List<Bullet>();
    private readonly List<Bullet> orphans = new List<Bullet>();
    private readonly Queue<Bullet> pooledObjects = new Queue<Bullet>();
    private Bullet bulletPrefab;

    public Bullet BulletPrefab 
    { 
        get { return bulletPrefab; }
        set 
        {
            for (int i = bullets.Count - 1; i > 0; i--)
            {
                if (pooledObjects.Contains(bullets[i]))
                {
                    Destroy(bullets[i].gameObject);
                }
                else
                {
                    orphans.Add(bullets[i]);
                }
                bullets.RemoveAt(i);
            }
            pooledObjects.Clear();
            bulletPrefab = value;
            for (int i = 0; i < amountToPool; i++)
            {
                Bullet bullet = Instantiate(bulletPrefab);
                bullet.gameObject.SetActive(false);
                pooledObjects.Enqueue(bullet);
                bullets.Add(bullet);
            }
        } 
    }

    public Bullet GetPooledObject(Vector3 position, Quaternion rotation)
    {
        if (pooledObjects.Count == 0)
        {
			Bullet newObject = Instantiate(bulletPrefab);
			pooledObjects.Enqueue(newObject);
			bullets.Add(newObject);
		}

		Bullet b = pooledObjects.Dequeue();
		b.transform.SetPositionAndRotation(position, rotation);
		b.gameObject.SetActive(true);
        b.ResetTrail();
        return b;
	}

    public void RepoolObject(Bullet bullet)
    {
        if (orphans.Contains(bullet))
        {
            Destroy(bullet.gameObject);
            return;
        }
        pooledObjects.Enqueue(bullet);
    }
}
