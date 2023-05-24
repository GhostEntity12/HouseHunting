using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] public AmmoInventory ammoInventory;
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] public GameObject muzzleFlashPrefab;

    //public GameObject shotgunPool, riflePool, crossbowPool;

    //bools
    private bool shooting, readyToShoot, reloading, aiming;

    private Animator anim;
    private Recoil recoil;

    public GunSO GunSO { get => gunSO; }
    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;

    public void Awake()
    {
        recoil = GetComponentInParent<Recoil>();
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

    }

    public void Aim()
    {
        if (!aiming)
        {
            aiming = true;
            anim.SetBool("Aiming", true);
        }

        else
        {
            aiming = false;
            anim.SetBool("Aiming", false);
        }
    }

    public void Shoot()
    {
        if (GameManager.Instance.IsPaused) return;
        if ( !readyToShoot || WeaponManager.Instance.BulletsInMag <= 0 || reloading) return;

        OnGunShootEvent?.Invoke();
        readyToShoot = false;

        //know which ammo pool to use
        switch(gunSO.name)
        {
            case "Shotgun":
                gunSO.bulletPrefab = ShotgunPool.SharedInstance.GetPooledObject(); 
                break;
            case "Rifle":
                gunSO.bulletPrefab = RiflePool.SharedInstance.GetPooledObject(); 
                break;
            case "Crossbow":
                gunSO.bulletPrefab = CrossbowPool.SharedInstance.GetPooledObject(); 
                break;
        }

        for (int i = 0; i < gunSO.bulletsPerTap; i++)
        {
            //Spread
            float x = Random.Range(-gunSO.spread, gunSO.spread);
            float y = Random.Range(-gunSO.spread, gunSO.spread);

            //reduce spread when aiming
            if (aiming)
            {
                x = x/4;
                y = y/4;
            }

            //calculate direction with spread
            Vector3 direction = Camera.main.transform.forward + new Vector3(x, y, 0);

            //Spawn bullet at attack point
            //GameObject currentBullet = Instantiate(gunSO.bulletPrefab, muzzlePoint.position, Quaternion.identity);
            //currentBullet.transform.forward = direction.normalized;
            //currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunSO.shootForce, ForceMode.Impulse);
            if (gunSO.bulletPrefab != null) 
            {
                gunSO.bulletPrefab.transform.position = muzzlePoint.position;
                gunSO.bulletPrefab.transform.rotation = Quaternion.identity;
                gunSO.bulletPrefab.SetActive(true);

                //Add force to bullet
                gunSO.bulletPrefab.GetComponent<Rigidbody>().AddForce(direction.normalized * gunSO.shootForce, ForceMode.Impulse);
            }
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
