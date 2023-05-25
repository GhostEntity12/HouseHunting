using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private List<Gun> allGuns;
    // private List<Gun> ownedGuns;
    private Gun currentGun;
    private int currentGunIndex = 0;
    private List<GunShopItem> ownedGuns = new();

    public List<Gun> OwnedGuns => allGuns.Where(x => ownedGuns.Any(y => y.id == x.GunSO.id)).ToList();
    public int CurrentGunIndex => currentGunIndex;
    public Gun CurrentGun => currentGun;
    public int BulletsInMag 
    {
        get => ownedGuns[currentGunIndex].bulletsInMag;
        set => ownedGuns[currentGunIndex].bulletsInMag = value;
    }

    public int BulletsInInventory => ownedGuns[currentGunIndex].totalBulletsLeft;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameManager.Instance.PermanentInventory.BoughtItems.ForEach(x => Debug.Log(x.GetType().ToString()));
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
        ownedGuns[currentGunIndex].totalBulletsLeft -= bulletsToReload;

        HuntingUIManager.Instance.SetAmmoCounterText(BulletsInMag / currentGun.GunSO.bulletsPerTap +  " / " + BulletsInInventory / currentGun.GunSO.bulletsPerTap);
    }
}
