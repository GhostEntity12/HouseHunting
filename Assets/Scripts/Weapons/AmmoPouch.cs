using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoPouch
{
	private Queue<Bullet> bulletPool = new();
	private Bullet bulletPrefab;

	public int AmmoStored { get; private set; }
	public int AmmoInGun { get; private set; }

	private void AddBullet()
	{
		Bullet bullet = Object.Instantiate(bulletPrefab);
		bullet.SetPool(this);
		bulletPool.Enqueue(bullet);
		bullet.gameObject.SetActive(false);
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
		bullet.gameObject.SetActive(false);
	}

	public Bullet DepoolBullet()
	{
		if (bulletPool.Count <= 1)
		{
			AddBullet();
		}
		Bullet bullet = bulletPool.Dequeue();
		bullet.Rigidbody.angularVelocity = Vector3.zero;
		bullet.Rigidbody.velocity = Vector3.zero;
		bullet.gameObject.SetActive(true);
		return bullet;
	}

	public int LoadGun(int capacity)
	{
		int ammoToLoad = Mathf.Min(AmmoStored, capacity - AmmoInGun);
		AmmoStored -= ammoToLoad;
		AmmoInGun += ammoToLoad;
		return AmmoInGun;
	}

	public void AddAmmo(int count)
	{
		AmmoStored += count;
	}

	public void RemoveAmmo(int count)
	{
		AmmoInGun -= count;
	}
}
