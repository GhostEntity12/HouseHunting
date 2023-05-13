using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunSO gunSO;
    [SerializeField] public AmmoInventory ammoInventory;
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] public GameObject muzzleFlashPrefab;

    private int ammoLeft, ammoShot;

    //bools
    private bool shooting, readyToShoot, reloading, aiming;

    //Reference
    private Camera cam;
    public AmmoType ammoType;

    private Animator anim;
    private Recoil Recoil_Script;

    public GunSO GunSO { get => gunSO; }
    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;


    public enum AmmoType
    {   
        shotgunAmmo,
        rifleAmmo,
        crossbowAmmo,
    }

    //Dictionary<AmmoType,int> ammoCount = new Dictionary<AmmoType,int>();

    public void Awake()
    {
        Recoil_Script = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
        ammoInventory = GameObject.Find("Player").GetComponent<AmmoInventory>();
        ammoLeft = gunSO.magSize;
        readyToShoot = true;
        anim = GetComponent<Animator>();
        aiming = false;
        cam = Camera.main;
    }


    private void Update()
    {
        MyInput();
        // Debug.Log(aiming);

        //Ammo Counter on HUD
        //text.SetText(ammoLeft / bulletsPerTap +  " / " + magSize / bulletsPerTap);
        switch(ammoType)
        {
            case AmmoType.shotgunAmmo:
                gunSO.ammoTotal = ammoInventory.shotgunAmmo;
                break;
            case AmmoType.rifleAmmo:
                gunSO.ammoTotal = ammoInventory.rifleAmmo;
                break;
            case AmmoType.crossbowAmmo:
                gunSO.ammoTotal = ammoInventory.crossbowAmmo;
                break;
        }

        HuntingUIManager.Instance.SetAmmoCounterText(ammoLeft / gunSO.bulletsPerTap +  " / " + gunSO.ammoTotal / gunSO.bulletsPerTap);
    }

    //private void OnDisable() => reloading = false;

    public void MyInput()
    {
        if (gunSO.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && ammoLeft < gunSO.magSize && !reloading && this.gameObject.activeSelf && gunSO.ammoTotal - gunSO.bulletsPerTap >= 0) 
        {
            ammoInventory.Spend(gunSO.magSize - ammoLeft);
            Reload();
        }
        

        //Shoot
        if (readyToShoot && shooting && !reloading &&  ammoLeft > 0)
        {
            ammoShot = gunSO.bulletsPerTap;
            Shoot();
        }

        //Aim
        if(Input.GetMouseButtonDown(1))
        {   
            aiming = true;
            anim.SetBool("Aiming", true);
        }

        if(Input.GetMouseButtonUp(1))
        {
            aiming = false;
            anim.SetBool("Aiming", false);
        }
    }


    private void Shoot()
    {
        OnGunShootEvent?.Invoke();
        readyToShoot = false;

        //Spread
        float x = Random.Range(-gunSO.spread, gunSO.spread);
        float y = Random.Range(-gunSO.spread, gunSO.spread);

        //calculate direction with spread
        if (aiming)
        {
            x = x/4;
            y = y/4;
        }

        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        //Spawn bullet at attack point
        Bullet currentBullet = Instantiate(gunSO.bulletPrefab, muzzlePoint.position, Quaternion.identity);
        
        currentBullet.transform.forward = direction.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunSO.shootForce, ForceMode.Impulse);

        //Muzzle flash
        Instantiate(muzzleFlashPrefab, muzzlePoint.position, Quaternion.identity);

        //recoil
        Recoil_Script.RecoilFire();

        ammoLeft--;
        ammoShot--;
        Invoke("ResetShot", gunSO.timeBetweenShooting);

        if (ammoShot > 0 && ammoLeft > 0)
        {
            Invoke("Shoot", gunSO.timeBetweenShots);
        }
    }


    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", gunSO.reloadTime);
    }

    private void ReloadFinished()
    {
        ammoLeft = gunSO.magSize;
        reloading = false;
    }

}
