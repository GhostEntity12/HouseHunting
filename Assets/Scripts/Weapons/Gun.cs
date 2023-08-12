using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject muzzleFlashPrefab;

    //bools
    private bool readyToShoot, reloading, aiming;

    private Recoil recoil;
    private SoundAlerter soundAlerter;

    public GunSO GunSO => gunSO;
    public Recoil Recoil => recoil;

    public void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
        readyToShoot = true;
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
            currentBullet.Damage = gunSO.damagePerBullet;
            currentBullet.CanBounce = GunSO.id.ToLower() == "crossbow";

            currentBullet.transform.forward = direction.normalized;

            //Add force to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunSO.shootForce, ForceMode.Impulse);
        }

        //Muzzle flash
        Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

        //recoil
        recoil.RecoilFire();

        WeaponManager.Instance.BulletsInMag -= gunSO.bulletsPerTap;
        HuntingUIManager.Instance.SetAmmoCounterText(WeaponManager.Instance.BulletsInMag / gunSO.bulletsPerTap +  " / " + WeaponManager.Instance.BulletsInInventory / gunSO.bulletsPerTap);

        StartCoroutine(ResetShot(gunSO.timeBetweenShots));
    }

    public void Reload()
    {
        if (reloading) return;
        if (HuntingInputManager.Instance.WeaponWheelIsOpen()) return; // dont reload while weapon wheel is open
        if (gunSO.magSize == WeaponManager.Instance.BulletsInMag) return; // dont reload when mag is full

        reloading = true;
        StartCoroutine(ResetReload(gunSO.reloadTime));
        HuntingUIManager.Instance.ReloadBarAnimation(gunSO.reloadTime);
    }

    private IEnumerator ResetReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        WeaponManager.Instance.Reload();
        reloading = false;
    }

    private IEnumerator ResetShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true;
    }
}
