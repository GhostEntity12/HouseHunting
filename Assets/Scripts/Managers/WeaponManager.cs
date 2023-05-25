using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private List<Gun> allGuns;
    private Gun currentGun;
    private int currentGunIndex = 0;
    private List<GunShopItem> ownedGuns = new();

    public int CurrentGunIndex => currentGunIndex;
    public Gun CurrentGun => currentGun;
    public List<Gun> AllGuns => allGuns;
    public int BulletsInMag 
    {
        get => ownedGuns[currentGunIndex].bulletsInMag;
        set => ownedGuns[currentGunIndex].bulletsInMag = value;
    }
    public int BulletsInInventory
    {
        get
        {
            ShopItem bulletShopItem = GameManager.Instance.PermanentInventory.BoughtItems.Find(x => x.id == currentGun.GunSO.bulletShopItem.id);
            if (bulletShopItem == null) return 0;
            return bulletShopItem.quantity;
        }
        set
        {
            ShopItem bulletShopItem = GameManager.Instance.PermanentInventory.BoughtItems.Find(x => x.id == currentGun.GunSO.bulletShopItem.id);
            // if null then the player doesn't have any bullets of this type and we need to add it to the inventory
            if (bulletShopItem == null)
            {
                bulletShopItem = new ShopItem(currentGun.GunSO.bulletShopItem.id, value);
                GameManager.Instance.PermanentInventory.BoughtItems.Add(bulletShopItem);
                return;
            }
            bulletShopItem.quantity = value;
        }
    }

    private void Start()
    {
        if (HuntingManager.Instance == null) return;

        ownedGuns = GameManager.Instance.PermanentInventory.BoughtItems.Where(x => x is GunShopItem).Cast<GunShopItem>().ToList();
        Gun firstOwnedGun = allGuns.Find(x => x.GunSO.id == ownedGuns[0].id);
        currentGun = Instantiate(firstOwnedGun, transform);
    }

    public void SelectItem(int index)
    {
        //TODO: make this compatible with other items than guns with interface
        Gun selectedGun = allGuns.Find(x => x.GunSO.id == ownedGuns[index].id);
        if (selectedGun != null && selectedGun != currentGun)
        {
            Destroy(currentGun.gameObject);
            currentGun = Instantiate(selectedGun, transform);
            currentGunIndex = index;
        }

        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap +  " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }

    public void Reload()
    {
        // if there are no bullets left, don't reload
        if (BulletsInInventory <= 0) return;

        int bulletsToReload = currentGun.GunSO.magSize - BulletsInMag;
        if (bulletsToReload > BulletsInInventory) bulletsToReload = BulletsInInventory;

        ownedGuns[currentGunIndex].bulletsInMag += bulletsToReload;
        BulletsInInventory -= bulletsToReload;

        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap +  " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }
}
