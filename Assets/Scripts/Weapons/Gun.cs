using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject muzzleFlashPrefab;

    //bools
    private bool readyToShoot, reloading, ads;
    private float elapsedTime; // for lerping ads

    private Vector3 initialPosition;
    private Recoil recoil;
    private SoundAlerter soundAlerter;
    private Animator anim;
    private Vector3 adsPosition;

    public GunSO GunSO => gunSO;
    public Recoil Recoil => recoil;


    private void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
        anim = GetComponent<Animator>();
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
        readyToShoot = true;
        ads = false;
        elapsedTime = 1;
        BulletPool.Instance.BulletPrefab = gunSO.bulletPrefab;
        initialPosition = transform.localPosition;
        adsPosition = new Vector3(initialPosition.x - 0.45f, initialPosition.y, initialPosition.z);
    }

    private void Update()
    {
        // for ADS animation
        elapsedTime += Time.deltaTime * 5;
        //Vector3 targetPosition = ads ? adsPosition : initialPosition;
        float cameraFov = ads ? 40 : 60;
        //transform.localPosition = Vector3.Lerp(ads ? initialPosition: adsPosition, targetPosition, elapsedTime);
        Camera.main.fieldOfView = Mathf.Lerp(ads ? 60 : 40, cameraFov, elapsedTime);

    }

    public void Shoot(bool firstShot = false)
    {
        if (GameManager.Instance.IsPaused) return;
        if ( !readyToShoot || WeaponManager.Instance.BulletsInMag <= 0 || reloading) return;
        if (HuntingInputManager.Instance.WeaponWheelIsOpen()) return; // dont shoot when weapon wheel is open

        if (firstShot)
            soundAlerter.MakeSound(GunSO.volume, transform.position);

        readyToShoot = false;
        //AnimationTrigger("Shoot"); // fire gun animation

        for (int i = 0; i < gunSO.bulletsPerTap; i++)
        {
            // calculate random spread
            float spread = Random.Range(-gunSO.spread, gunSO.spread);

            // decrease spread if ads is active
            if (ads) spread /= 4;

            // get bullet at muzzle point
            Bullet currentBullet = BulletPool.Instance.GetPooledObject(muzzlePoint.position, Quaternion.identity);
            currentBullet.Damage = gunSO.damagePerBullet;
            currentBullet.CanBounce = GunSO.id.ToLower() == "crossbow";
            
            // cast ray to crosshair, if the ray hits something, means that there are something in range that should be aimed that, otherwise, the direction can be slightly off
            // currently, this change still does not fix the problem completely because the colliders on trees are weird
            // so if you aim at a spot where there are no colliders present on the tree (such as very high up), the bullet will still not go to where the crosshair aims at because the raycast could not find a target
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentBullet.transform.LookAt(hit.point);
                currentBullet.transform.forward += new Vector3(spread, spread, 0);
            }
            else // if the ray doesnt hit anything, means that the target the player is trying to aim is too far away, and will not hit anything anyways. So we could just apply the forward direction of the camera to the bullet.
            {
                Vector3 direction = Camera.main.transform.forward + new Vector3(spread, spread, spread);
                currentBullet.transform.forward = direction.normalized;
            }

            //Add force to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward.normalized * gunSO.shootForce, ForceMode.Impulse);
        }

        //Muzzle flash
        Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

        //recoil
        recoil.RecoilFire();

        WeaponManager.Instance.BulletsInMag -= gunSO.bulletsPerTap;
        HuntingUIManager.Instance.SetAmmoCounterText(WeaponManager.Instance.BulletsInMag / gunSO.bulletsPerTap +  " / " + WeaponManager.Instance.BulletsInInventory / gunSO.bulletsPerTap);

        StartCoroutine(ResetShot(gunSO.timeBetweenShots));
    }

    // so outside managers can trigger animations if needed
    public void AnimationTrigger(string animationName)
    {
        anim.SetTrigger(animationName);
    }

    public void Reload()
    {
        if (reloading) return;
        if (HuntingInputManager.Instance.WeaponWheelIsOpen()) return; // dont reload while weapon wheel is open
        if (gunSO.magSize == WeaponManager.Instance.BulletsInMag) return; // dont reload when mag is full

        AnimationTrigger("Reload");

        
        reloading = true;
        ToggleADS(false);
        anim.SetBool("Reload", reloading);
        StartCoroutine(ResetReload(gunSO.reloadTime));
        HuntingUIManager.Instance.ReloadBarAnimation(gunSO.reloadTime);
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

    private IEnumerator ResetReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        WeaponManager.Instance.Reload();
        reloading = false;
        anim.SetBool("Reload", reloading);
    }

    private IEnumerator ResetShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true;
    }
}
