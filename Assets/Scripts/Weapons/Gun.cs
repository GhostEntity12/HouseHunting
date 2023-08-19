using System.Collections;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] public GameObject muzzleFlashPrefab;

    //bools
    private bool shooting, readyToShoot, reloading, aiming;

    private Animator anim;
    private Recoil recoil;
    private SoundAlerter soundAlerter;

    public GunSO GunSO => gunSO;

    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;

    public void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
        readyToShoot = true;
        anim = GetComponent<Animator>();
        aiming = false;
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
            float x = Random.Range(-gunSO.spread, gunSO.spread);
            float y = Random.Range(-gunSO.spread, gunSO.spread);

            //calculate direction with spread
            if (aiming)
            {
                x = x/4;
                y = y/4;
            }

            Vector3 direction = Camera.main.transform.forward + new Vector3(x, y, 0);

            //Spawn bullet at attack point
            Bullet currentBullet = Instantiate(gunSO.bulletPrefab, muzzlePoint.position, Quaternion.identity);
            
            currentBullet.transform.forward = direction.normalized;

            //Add force to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunSO.shootForce, ForceMode.Impulse);
        }

        OnGunShootEvent?.Invoke();

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
