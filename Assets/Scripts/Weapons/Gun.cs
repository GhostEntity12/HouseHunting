using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
	enum GunState { Ready, Shooting, Reloading }
	
	[SerializeField] private Transform muzzlePoint;
	[SerializeField] private GameObject muzzleFlashPrefab;
	[SerializeField] private float adsFactor = 4;

	private GunState state = GunState.Ready;
	private bool usingADS = false;
	private Animator anim;
	private Recoil recoil;
	private SoundAlerter soundAlerter;

	[field: SerializeField] public GunSO GunSO { get; private set; }
	public int LoadedAmmunition {get; private set;}

	public delegate void OnGunShoot();
	public static event OnGunShoot OnGunShootEvent;

	public void Awake()
	{
		recoil = GetComponentInParent<Recoil>();
		soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
		anim = GetComponent<Animator>();
	}

	private void ReenableGun() => state = GunState.Ready;

	public void Shoot()
	{
		if (GameManager.Instance.IsPaused) return;
		if (state != GunState.Ready || LoadedAmmunition <= 0) return; // WeaponManager.Instance.BulletsInMag <= 0) return;

		// Set state to shooting
		state = GunState.Shooting;

		// Call event
		OnGunShootEvent?.Invoke();

		int bulletsToFire = Mathf.Max(GunSO.bulletsPerTap, LoadedAmmunition);
		// Instantiate bullets
		for (int i = 0; i < bulletsToFire; i++)
		{
			// Add local inaccuracy
			Vector3 spread = new(Random.Range(-GunSO.spread, GunSO.spread), Random.Range(-GunSO.spread, GunSO.spread), 0);

			// If using ADS, reduce inaccuracy
			if (usingADS)
				spread /= adsFactor;

			// Calculate world direction 
			Vector3 direction = Camera.main.transform.forward + spread;

			// Instantiate at muzzle point
			Bullet currentBullet = Instantiate(GunSO.bulletPrefab, muzzlePoint.position, Quaternion.identity);
			currentBullet.transform.forward = direction.normalized;

			// Add force to bullet
			currentBullet.Rigidbody.AddForce(direction.normalized * GunSO.shootForce, ForceMode.Impulse);
		}
		// Remove bullets
		LoadedAmmunition -= bulletsToFire;

		// Update UI
		HuntingUIManager.Instance.SetAmmoCounterText($"{bulletsToFire / GunSO.bulletsPerTap} / {WeaponManager.Instance.BulletsInInventory / GunSO.bulletsPerTap}");

		//Muzzle flash
		Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

		// Recoil
		recoil.RecoilFire();

		// Make noise
		soundAlerter.MakeSound(GunSO.volume, transform.position);

		// Reenable gun after wait
		Invoke(nameof(ReenableGun), GunSO.timeBetweenShots);
	}

	public void Reload()
	{
		// Skip if the gun can't be fired yet or if already at max ammo
		if (state != GunState.Ready || LoadedAmmunition == GunSO.magSize) return;

		// Set the state to reloading and reenable it after a wait
		state = GunState.Reloading;
		Invoke(nameof(ReenableGun), GunSO.reloadTime);

		// Reload the gun
		LoadedAmmunition = WeaponManager.Instance.GetAmmoForReload(this);
		// Update UI
		HuntingUIManager.Instance.SetAmmoCounterText($"{LoadedAmmunition / GunSO.bulletsPerTap} / {WeaponManager.Instance.BulletsInInventory / GunSO.bulletsPerTap}");
	}
}
