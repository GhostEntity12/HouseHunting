using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    
    public int shotgunAmmo, rifleAmmo, crossbowAmmo;
    
    public GameObject weaponHolder, shotgun, lar, xbow;

    public void addShotgunAmmo(int ammoAdded)
    {
        shotgunAmmo += ammoAdded;
    }

    public void addRifleAmmo(int ammoAdded)
    {
        rifleAmmo += ammoAdded;
    }

    public void addCrossbowAmmo(int ammoAdded)
    {
        crossbowAmmo += ammoAdded;
    }


    public void Spend(int ammoSpent)
    {   
        if (shotgun.activeInHierarchy)
        {
            shotgunAmmo -= ammoSpent;
        }

        if (lar.activeInHierarchy)
        {
            rifleAmmo -= ammoSpent; 
        }

        if (xbow.activeInHierarchy)
        {
            crossbowAmmo -= ammoSpent;   
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        shotgun = weaponHolder.transform.GetChild(0).gameObject;
        lar = weaponHolder.transform.GetChild(1).gameObject;
        xbow = weaponHolder.transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
