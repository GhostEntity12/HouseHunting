using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
	private readonly List<Gun> allGuns = new();
	private readonly List<Gun> ownedGuns = new();

	public int CurrentGunIndex { get; private set; } = 0;
	public Gun CurrentGun { get; private set; } = null;

	private void Start()
	{
        // Return if not in Hunting scene
        if (HuntingManager.Instance == null) return;

        // Iterating over children
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Gun g))
            {
                allGuns.Add(g);
                if (GameManager.Instance.OwnedGuns.Select(g => g.id).Any(i => g.GunSO.id == i))
                {
                    ownedGuns.Add(g);
                    // Temp to give 5x ammo at start
                    g.AmmoPouch.AddAmmo(g.GunSO.magSize * 5);
                    // Works as in instant reload
                    g.AmmoPouch.LoadGun(g.GunSO.magSize);
                }
                g.gameObject.SetActive(false);
            }
        }
        SelectItem(CurrentGunIndex);
	}

	public void SelectItem(int index)
	{
        if (index >= ownedGuns.Count) return;

        //TODO: make this compatible with other items than guns with interface
        Gun selectedGun = ownedGuns[index];

        if (CurrentGun == null)
            CurrentGun = selectedGun; // make sure theres a gun to check id of

        // if the selected gun is different to the previous gun, disable old gun prefab
        if (selectedGun != null && selectedGun.GunSO.id != CurrentGun.GunSO.id)
        {
            CurrentGun.gameObject.SetActive(false);
        }
        CurrentGun = selectedGun; // swap
        selectedGun.gameObject.SetActive(true); // enable
        SoundAlerter.MakeSound(CurrentGun.GunSO.equipSound, transform.position);
        HuntingUIManager.Instance.AmmoUI.Rerender(selectedGun);
    }
 
    // function which gives the player ammo
    public void GiveAmmo(int number)
    {
        CurrentGun.AmmoPouch.AddAmmo(number);
    }
}
