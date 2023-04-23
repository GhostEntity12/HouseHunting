using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Gun : MonoBehaviour
{
    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;


    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;
    int ammoLeft, ammoShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera Cam;
    public RaycastHit hit;
    public Transform muzzlePoint;

    //Graphics
    //public CameraShake camShake;
    public GameObject muzzleFlash, bulletHole;
    public TextMeshProUGUI text;


    
    
    private void Awake()
    {
        ammoLeft = magSize;
        readyToShoot = true;
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

        //Shoot
        if (readyToShoot && shooting && !reloading &&  ammoLeft > 0)
        {
            ammoShot = bulletsPerTap;
            Shoot();
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
        
        //Raycast
        if (Physics.Raycast(Cam.transform.position, direction, out hit, range))
        {
            Shootable shootableTarget = hit.transform.GetComponentInParent<Shootable>();
            if (shootableTarget != null)
            {
                shootableTarget.TakeDamage(damage);
            }
        }

        //Camera shake
        //camShake.Shake(camShakeDuration, camShakeMagnitude);

        //bullet hole
        GameObject obj = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
        obj.transform.position += obj.transform.forward/1000;

        //Muzzle flash
        Instantiate(muzzleFlash, muzzlePoint.position, Quaternion.identity);

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
