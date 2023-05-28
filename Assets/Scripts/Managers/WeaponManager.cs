using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private List<Gun> guns;

    private Gun currentGun;
    private int currentGunIndex = 0;
    private List<WeaponInventoryItem> gunAmmo = new();
    private SoundAlerter soundAlerter;

    public List<Gun> Guns => guns;
    public int CurrentGunIndex => currentGunIndex;
    public Gun CurrentGun => currentGun;
    public int BulletsInInventory => gunAmmo[currentGunIndex].totalBulletsLeft;
    public int BulletsInMag 
    {
        get => gunAmmo[currentGunIndex].bulletsInMag;
        set => gunAmmo[currentGunIndex] = new WeaponInventoryItem(gunAmmo[currentGunIndex].id, value, gunAmmo[currentGunIndex].totalBulletsLeft);
    }

    protected override void Awake()
    {
        base.Awake();

        currentGun = Instantiate(guns[0], transform);
    }

    private void Start()
    {
        gunAmmo = GameManager.Instance.PermanentInventory.GunAmmo;
        soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();

        foreach (Gun gun in guns)
        {
            // if the gun is not in the inventory, add it
            if (string.IsNullOrEmpty(gunAmmo.Find(x => x.id == gun.GunSO.id).id))
            {
                gunAmmo.Add(new WeaponInventoryItem(gun.GunSO.id));
            }
        }
    }

    public void SelectGun(int index)
    {
        if (index >= Guns.Count)
            return;
        Gun selectedGun = guns[index];
        if (selectedGun != null && selectedGun != currentGun)
        {
            Destroy(currentGun.gameObject);
            currentGun = Instantiate(selectedGun, transform);
            currentGunIndex = index;
            soundAlerter.MakeSound(10, transform.position);
        }

        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap +  " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }
 
    // function which gives the player ammo
    public void GiveAmmo(int number)
    {
        gunAmmo[currentGunIndex] = new WeaponInventoryItem(gunAmmo[currentGunIndex].id, gunAmmo[currentGunIndex].bulletsInMag, gunAmmo[currentGunIndex].totalBulletsLeft + number);
        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap + " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }

    public void Reload()
    {
        // if there are no bullets left, don't reload
        if (BulletsInInventory <= 0) return;

        int bulletsToReload = currentGun.GunSO.magSize - BulletsInMag;
        if (bulletsToReload > BulletsInInventory) bulletsToReload = BulletsInInventory;

        gunAmmo[currentGunIndex] = new WeaponInventoryItem(gunAmmo[currentGunIndex].id, gunAmmo[currentGunIndex].bulletsInMag + bulletsToReload, gunAmmo[currentGunIndex].totalBulletsLeft - bulletsToReload);

        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap +  " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }
}
