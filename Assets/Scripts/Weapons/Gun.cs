using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;

    //bullet
    public GameObject bullet;
    public float shootForce;

    //Gun stats
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;
    int ammoLeft, ammoShot;


    //bools
    bool shooting, readyToShoot, reloading, aiming;

    //Reference
    public Camera Cam;
    public Transform muzzlePoint;
    public Animator anim;
    public GameObject ammoType1, ammoType2;


    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI text;
    

    private Recoil Recoil_Script;

    public void Awake()
    {
        Recoil_Script = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
        ammoLeft = magSize;
        readyToShoot = true;
        anim = GetComponent<Animator>();
        aiming = false;
    }


    private void Update()
    {
        MyInput();

        //Ammo Counter on HUD
        text.SetText(ammoLeft +  " / " + magSize);
    }

    //private void OnDisable() => reloading = false;

    public void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && ammoLeft < magSize && !reloading && this.gameObject.activeSelf) Reload();

        //Switching ammo
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bullet = ammoType1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bullet = ammoType2;
        }

        //Shoot
        if (readyToShoot && shooting && !reloading &&  ammoLeft > 0)
        {
            ammoShot = bulletsPerTap;
            Shoot();
        }

        //Aim
            //Very inefficient
        // TODO
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
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //calculate direction with spread
        Vector3 direction = Cam.transform.forward + new Vector3(x, y, 0);

        //Spawn bullet at attack point
        GameObject currentBullet = Instantiate(bullet, muzzlePoint.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);

        //Muzzle flash
        Instantiate(muzzleFlash, muzzlePoint.position, Quaternion.identity);

        //recoil
        Recoil_Script.RecoilFire();

        ammoLeft--;
        ammoShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (ammoShot > 0 && ammoLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

    }


    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        ammoLeft = magSize;
        reloading = false;
    }

}
