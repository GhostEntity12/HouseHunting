using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public AmmoInventory ammoInv_Script;

    public enum AmmoType
    {   
        shotgunAmmo,
        rifleAmmo,
        crossbowAmmo,
    }

    public AmmoType ammoType;

    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch (ammoType)
            {
                case AmmoType.shotgunAmmo:
                    ammoInv_Script.addShotgunAmmo(48);
                    Destroy(gameObject);
                    break;
                case AmmoType.rifleAmmo:
                    ammoInv_Script.addRifleAmmo(7);
                    Destroy(gameObject);
                    break;
                case AmmoType.crossbowAmmo:
                    ammoInv_Script.addCrossbowAmmo(3);
                    Destroy(gameObject);
                    break;

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ammoInv_Script = GameObject.Find("Player").GetComponent<AmmoInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
