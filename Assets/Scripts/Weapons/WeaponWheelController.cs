using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelController : MonoBehaviour
{
    public Animator anim;
    public bool weaponWheelSelected = false;
    public Image selectedItem;
    public Sprite noImage;
    public static int weaponID;
    public GameObject weaponHolder, SSG, LAR, Xbow;
    private Recoil Recoil_Script;



    void Start()
    {
        Recoil_Script = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
        SSG = weaponHolder.transform.GetChild(0).gameObject;
        LAR = weaponHolder.transform.GetChild(1).gameObject;
        Xbow = weaponHolder.transform.GetChild(2).gameObject;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelSelected = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }


        if(weaponWheelSelected)
        {
            
            anim.SetBool("OpenWeaponWheel", true);
        }
        else
        {
            anim.SetBool("OpenWeaponWheel", false);
        }

        switch(weaponID)
        {
            case 0: //no weapon selected
                selectedItem.sprite = noImage;
                break;

            case 1: //SSG
                SSG.SetActive(true);
                LAR.SetActive(false);
                Xbow.SetActive(false);
                Recoil_Script.recoilX = 1;
                Recoil_Script.recoilY = 1;
                Recoil_Script.recoilZ = 1;
                Recoil_Script.snappiness = 3;
                Recoil_Script.returnSpeed = 2;
                weaponWheelSelected = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.Confined;
                weaponID = 0;
                break;

            case 2: //LAR
                SSG.SetActive(false);
                LAR.SetActive(true);
                Xbow.SetActive(false);
                Recoil_Script.recoilX = 14;
                Recoil_Script.recoilY = 14;
                Recoil_Script.recoilZ = 14;
                Recoil_Script.snappiness = 25;
                Recoil_Script.returnSpeed = 5;
                weaponWheelSelected = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.Confined;
                weaponID = 0;
                break;
            
            case 3: //Xbow
                SSG.SetActive(false);
                LAR.SetActive(false);
                Xbow.SetActive(true);
                Recoil_Script.recoilX = 1;
                Recoil_Script.recoilY = 1;
                Recoil_Script.recoilZ = 1;
                Recoil_Script.snappiness = 1;
                Recoil_Script.returnSpeed = 2;
                weaponWheelSelected = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.Confined;
                weaponID = 0;
                break;
        }
    }
}
