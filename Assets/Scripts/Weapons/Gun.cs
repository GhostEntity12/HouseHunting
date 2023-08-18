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
	[field: SerializeField] public AmmoPouch AmmoPouch { get; private set; } = new();

	public string AmmoInfo => $"{AmmoPouch.AmmoInGun / GunSO.bulletsPerTap} / {AmmoPouch.AmmoStored / GunSO.bulletsPerTap}";

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
		if (state != GunState.Ready || AmmoPouch.AmmoInGun <= 0) return;

		// Set state to shooting
		state = GunState.Shooting;

		// If gun is not fuly loaded, only fire bullets in gun
		int bulletsToFire = Mathf.Min(GunSO.bulletsPerTap, AmmoPouch.AmmoInGun);
        AnimationTrigger("Shoot"); // fire gun animation

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
		AmmoPouch.RemoveAmmo(bulletsToFire);

		// Update UI
		HuntingUIManager.Instance.SetAmmoCounterText(AmmoInfo);

		//Muzzle flash
		Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

		// Recoil
		recoil.RecoilFire();

		// Make noise
		soundAlerter.MakeSound(GunSO.volume, transform.position);

		// Reenable gun after wait
		Invoke(nameof(ReenableGun), GunSO.timeBetweenShots);
	}

	// so outside managers can trigger animations if needed
	public void AnimationTrigger(string animationName)
	{
		anim.SetTrigger(animationName);
	}

	public void Reload()
	{
		// Skip if the gun can't be fired yet, no ammo in pouch or if already at max ammo
		if (state != GunState.Ready || AmmoPouch.AmmoStored == 0 || AmmoPouch.AmmoInGun == GunSO.magSize) return;

		// Set the state to reloading and reenable it after a wait
		state = GunState.Reloading;
		anim.SetBool("Reload", true);
		AnimationTrigger("Reload");
		Invoke(() => {
			ReenableGun();
			anim.SetBool("Reload", false);
		},
			GunSO.reloadTime);

		// Reload the gun
		AmmoPouch.LoadGun(GunSO.magSize);

		// Update UI
		HuntingUIManager.Instance.SetAmmoCounterText(AmmoInfo);
	}
}
