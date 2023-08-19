using System.Collections;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

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

    public GunSO GunSO => gunSO;
    public Recoil Recoil => recoil;

    private void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
        readyToShoot = true;
        ads = false;
        elapsedTime = 1;
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // for ADS animation
        elapsedTime += Time.deltaTime;
        Vector3 adsPosition = new Vector3(initialPosition.x - 0.45f, initialPosition.y, initialPosition.z);
        Vector3 targetPosition = ads ? adsPosition : initialPosition;
        float cameraFov = ads ? 40 : 60;
        transform.localPosition = Vector3.Lerp(ads ? initialPosition: adsPosition, targetPosition, elapsedTime / 0.2f);
        Camera.main.fieldOfView = Mathf.Lerp(ads ? 60 : 40, cameraFov, elapsedTime / 0.2f);

    }

    public void Shoot(bool firstShot = false)
    {
        if (GameManager.Instance.IsPaused) return;
        if ( !readyToShoot || WeaponManager.Instance.BulletsInMag <= 0 || reloading) return;
        if (HuntingInputManager.Instance.WeaponWheelIsOpen()) return; // dont shoot when weapon wheel is open

        if (firstShot)
            soundAlerter.MakeSound(GunSO.volume, transform.position);

        readyToShoot = false;
        AnimationTrigger("Shoot"); // fire gun animation

        for (int i = 0; i < gunSO.bulletsPerTap; i++)
        {
            //Spread
            float spread = Random.Range(-gunSO.spread, gunSO.spread);

            //calculate direction with spread
            if (ads) spread /= 4;

            Vector3 direction = Camera.main.transform.forward + new Vector3(spread, spread, 0);

            //Spawn bullet at muzzle point
            Bullet currentBullet = Instantiate(gunSO.bulletPrefab, muzzlePoint.position, Quaternion.identity);
            currentBullet.Damage = gunSO.damagePerBullet;
            currentBullet.CanBounce = GunSO.id.ToLower() == "crossbow";

            currentBullet.transform.forward = direction.normalized;
            
            // cast ray to crosshair, if the ray hits something, means that there are something in range that should be aimed that, otherwise, the direction can be slightly off
            // currently, this change still does not fix the problem completely because the colliders on trees are weird
            // so if you aim at a spot where there are no colliders present on the tree (such as very high up), the bullet will still not go to where the crosshair aims at because the raycast could not find a target
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentBullet.transform.LookAt(hit.point);
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
        anim.SetBool("Reload", reloading);
        StartCoroutine(ResetReload(gunSO.reloadTime));
        HuntingUIManager.Instance.ReloadBarAnimation(gunSO.reloadTime);
    }
    public void ToggleADS()
    {
        ads = !ads;
        elapsedTime = 0;
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
