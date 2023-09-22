using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AmmoPouch
{
	private Queue<Bullet> bulletPool = new();
	private Bullet bulletPrefab;
	public int AmmoStored { get; private set; }
	public int AmmoInGun { get; private set; }

	void AddBullet()
	{
		Bullet bullet = GameObject.Instantiate(bulletPrefab);
		bullet.SetPool(this);
		PoolBullet(bullet);
	}

	public void SetupPool(int poolSize, Bullet prefab)
	{
		bulletPrefab = prefab;
		for (int i = 0; i < poolSize; i++)
		{
			AddBullet();
		}
	}

	public void PoolBullet(Bullet bullet)
	{
		bulletPool.Enqueue(bullet);
		bullet.ResetTrail();
		bullet.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		bullet.Rigidbody.angularVelocity = Vector3.zero;
		bullet.Rigidbody.velocity = Vector3.zero;
		bullet.gameObject.SetActive(false);
	}

	public Bullet DepoolBullet(Vector3 pos, Quaternion rot)
	{
		if (bulletPool.Count > 0)
		{
			AddBullet();
		}
		Bullet b = bulletPool.Dequeue();
		b.gameObject.SetActive(true);
		b.transform.SetPositionAndRotation(pos, rot);
		return b;
	}


	public int LoadGun(int capacity)
	{
		int ammoToLoad = capacity - AmmoInGun;
		AmmoStored -= ammoToLoad;
		AmmoInGun += ammoToLoad;
		return AmmoInGun;
	}

	public void AddAmmo(int count) => AmmoStored += count;
	public void RemoveAmmo(int count) => AmmoInGun -= count;

}
