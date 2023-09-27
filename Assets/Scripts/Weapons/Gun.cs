using UnityEngine;

public class Gun : MonoBehaviour, IEquippable
{
	enum GunState { Ready, Shooting, Reloading }

	[SerializeField] private Transform muzzlePoint;
	[SerializeField] private GameObject muzzleFlashPrefab;
	[SerializeField] private float adsFactor = 4;

	private bool ads;
	private float elapsedTime; // for lerping ads

	private GunState state = GunState.Ready;
	private Animator anim;
	private Vector3 initialPosition;
	private Vector3 adsPosition;

	[field: SerializeField] public GunSO GunSO { get; private set; }
	[field: SerializeField] public AmmoPouch AmmoPouch { get; private set; } = new();

	public string AmmoInfo => $"{AmmoPouch.AmmoInGun / GunSO.bulletsPerTap} / {AmmoPouch.AmmoStored / GunSO.bulletsPerTap}";
    public int NumberOfMagazineLeft => AmmoPouch.AmmoStored / GunSO.bulletsPerTap;
    public int NumberOfShotsLeft => AmmoPouch.AmmoInGun / GunSO.bulletsPerTap;
    public int MaxShotPerMagazine => GunSO.magSize / GunSO.bulletsPerTap;

	public string ID => GunSO.id;

	public SoundAlertSO EquipSound => GunSO.equipSound;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		ads = false;
		elapsedTime = 1;
		initialPosition = transform.localPosition;
		adsPosition = new Vector3(initialPosition.x - 0.45f, initialPosition.y, initialPosition.z);
	}

	private void ReenableGun() => state = GunState.Ready;

	private void Update()
	{
		// for ADS animation
		elapsedTime += Time.deltaTime * 5;
		//Vector3 targetPosition = ads ? adsPosition : initialPosition;
		float cameraFov = ads ? 40 : 60;
		//transform.localPosition = Vector3.Lerp(ads ? initialPosition: adsPosition, targetPosition, elapsedTime);
		Camera.main.fieldOfView = Mathf.Lerp(ads ? 60 : 40, cameraFov, elapsedTime);
	}

	public void Shoot()
	{
		if (state != GunState.Ready || AmmoPouch.AmmoInGun <= 0) return;

		// Set state to shooting
		state = GunState.Shooting;

		// If gun is not fuly loaded, only fire bullets in gun
		int bulletsToFire = Mathf.Min(GunSO.bulletsPerTap, AmmoPouch.AmmoInGun);

		AnimationTrigger("Shoot"); // fire gun animation

		for (int i = 0; i < GunSO.bulletsPerTap; i++)
		{
			// calculate random spread
			Vector3 spread = new(Random.Range(-GunSO.spread, GunSO.spread), Random.Range(-GunSO.spread, GunSO.spread), 0);

			// decrease spread if ads is active
			if (ads) spread /= 4;

			// get bullet at muzzle point
			Bullet currentBullet = AmmoPouch.DepoolBullet();
			currentBullet.transform.position = muzzlePoint.position;
			currentBullet.ResetTrail();
			currentBullet.Damage = GunSO.damagePerBullet;
			currentBullet.CanBounce = GunSO.id.ToLower() == "crossbow";

			//cast ray to crosshair, if the ray hits something, means that there are something in range that should be aimed that, otherwise, the direction can be slightly off
			//currently, this change still does not fix the problem completely because the colliders on trees are weird
			//so if you aim at a spot where there are no colliders present on the tree(such as very high up), the bullet will still not go to where the crosshair aims at because the raycast could not find a target
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				currentBullet.transform.LookAt(hit.point);
				currentBullet.transform.forward += spread;
			}
			else // if the ray doesnt hit anything, means that the target the player is trying to aim is too far away, and will not hit anything anyways. So we could just apply the forward direction of the camera to the bullet.
			{
				Vector3 direction = Camera.main.transform.forward + spread;
				currentBullet.transform.forward = direction.normalized;
			}

			//Add force to bullet
			currentBullet.Rigidbody.AddForce(currentBullet.transform.forward.normalized * GunSO.shootForce, ForceMode.Impulse);
		}

		AudioManager.Instance.Play(GunSO.name);

		// Remove bullets
		AmmoPouch.RemoveAmmo(bulletsToFire);

		// Update UI
        HuntingUIManager.Instance.AmmoUI.SetBulletsLeft(NumberOfShotsLeft);

		//Muzzle flash
		Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

		// Make noise
		SoundAlerter.MakeSound(GunSO.fireSound, transform.position);

		// Reenable gun after wait
		Invoke(nameof(ReenableGun), GunSO.timeBetweenShots);
	}

	// so outside managers can trigger animations if needed
	public void AnimationTrigger(string animationName) => anim.SetTrigger(animationName);

	public void Reload()
	{
		// Skip if the gun can't be fired yet, no ammo in pouch or if already at max ammo
		if (state != GunState.Ready || AmmoPouch.AmmoStored == 0 || AmmoPouch.AmmoInGun == GunSO.magSize) return;

		AudioManager.Instance.Play(GunSO.name + " Reload");

		// Trigger reload
		state = GunState.Reloading;
		if(ads)
			ToggleADS(false);
		anim.SetBool("Reload", true);
		AnimationTrigger("Reload");
        //HuntingUIManager.Instance.ReloadBarAnimation(GunSO.reloadTime);
        HuntingUIManager.Instance.AmmoUI.isReloading = true;
        HuntingUIManager.Instance.AmmoUI.reloadTime = GunSO.reloadTime;
		AmmoUI.OnReloadFinishEvent += OnReloadFinish;
	}

	public void ToggleADS()
	{
		ToggleADS(!ads);
	}

	public void ToggleADS(bool isAds)
	{
		ads = isAds;
		elapsedTime = 0;
		anim.SetBool("Aiming", ads);
	}

	private void OnReloadFinish()
	{
		AmmoPouch.LoadGun(GunSO.magSize);
		anim.SetBool("Reload", false);

		// Reload the gun
		AmmoUI.OnReloadFinishEvent -= OnReloadFinish;
		// Update UI
		SoundAlerter.MakeSound(GunSO.reloadSound, transform.position);

		HuntingUIManager.Instance.AmmoUI.Rerender(this);
		ReenableGun();
	}

	public void Equip()
	{
		gameObject.SetActive(true);
		HuntingUIManager.Instance.AmmoUI.Rerender(this);
	}

	public void Unequip()
	{
		gameObject.SetActive(false);
	}

	public void UsePrimary() => Shoot();

	public void UseSecondary() => ToggleADS();
}
