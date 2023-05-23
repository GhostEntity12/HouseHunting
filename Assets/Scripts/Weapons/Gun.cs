using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] public AmmoInventory ammoInventory;
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] public GameObject muzzleFlashPrefab;

    //bools
    private bool shooting, readyToShoot, reloading, aiming;

    private Animator anim;
    private Recoil recoil;
    private SoundAlerter soundAlerter;

    public GunSO GunSO { get => gunSO; }
    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;

    //Dictionary<AmmoType,int> ammoCount = new Dictionary<AmmoType,int>();

    public void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
        ammoInventory = GameObject.Find("Player").GetComponent<AmmoInventory>();
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
        readyToShoot = true;
        anim = GetComponent<Animator>();
        aiming = false;
    }


    private void Update()
    {
        // switch(ammoType)
        // {
        //     case AmmoType.shotgunAmmo:
        //         gunSO.ammoTotal = ammoInventory.shotgunAmmo;
        //         break;
        //     case AmmoType.rifleAmmo:
        //         gunSO.ammoTotal = ammoInventory.rifleAmmo;
        //         break;
        //     case AmmoType.crossbowAmmo:
        //         gunSO.ammoTotal = ammoInventory.crossbowAmmo;
        //         break;
        // }
    }

    public void MyInput()
    {
        if (GameManager.Instance.IsPaused) return;
        if (gunSO.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);

        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < gunSO.magSize && !reloading && this.gameObject.activeSelf && gunSO.ammoTotal - gunSO.bulletsPerTap >= 0) 
        // {
        //     ammoInventory.Spend(gunSO.magSize - bulletsLeft);
        //     Reload();
        // }
        
        //Shoot
        // if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        // {
        //     ammoShot = gunSO.bulletsPerTap;
        //     Shoot();
        // }

        // //Aim
        // if(Input.GetMouseButtonDown(1))
        // {   
        //     aiming = true;
        //     anim.SetBool("Aiming", true);
        // }

        // if(Input.GetMouseButtonUp(1))
        // {
        //     aiming = false;
        //     anim.SetBool("Aiming", false);
        // }
    }

    public void Shoot(bool firstShot = false)
    {
        if (GameManager.Instance.IsPaused) return;
        if ( !readyToShoot || WeaponManager.Instance.BulletsInMag <= 0 || reloading) return;

        if (firstShot)
        {
            soundAlerter.MakeSound(volume, transform.position);
        }
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

        reloading = true;
        StartCoroutine(ResetReload(gunSO.reloadTime));
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
