using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Singleton<BulletPool>
{
    [SerializeField] private int amountToPool;

    private readonly List<Bullet> pooledObjects = new List<Bullet>();
    private Bullet bulletPrefab;

    public Bullet BulletPrefab 
    { 
        get { return bulletPrefab; }
        set 
        {
            foreach(Bullet bullet in pooledObjects)
            {
                Destroy(bullet.gameObject);
            }
            pooledObjects.Clear();
            bulletPrefab = value;
            for (int i = 0; i < amountToPool; i++)
            {
                Bullet bullet = Instantiate(bulletPrefab);
                bullet.gameObject.SetActive(false);
                pooledObjects.Add(bullet);
            }
        } 
    }

    public Bullet GetPooledObject(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].transform.position = position;
                pooledObjects[i].transform.rotation = rotation;
                pooledObjects[i].gameObject.SetActive(true);
                return pooledObjects[i];
            }
        }

        Bullet newObject = Instantiate(bulletPrefab);
        pooledObjects.Add(newObject);
        return pooledObjects[0];
    }
}
