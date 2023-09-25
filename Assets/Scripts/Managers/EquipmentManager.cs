using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentManager : Singleton<EquipmentManager>
{
	private readonly List<IEquippable> usableEquipment = new();

	public int EquippedItemIndex { get; private set; } = 0;
	public IEquippable EquippedItem { get; private set; } = null;

	private void Start()
	{
		// Return if not in Hunting scene
		if (HuntingManager.Instance == null) return;

		// Iterating over children
		foreach (Transform t in transform)
		{
			if (t.TryGetComponent(out IEquippable e))
			{
				usableEquipment.Add(e);
				if ((e is Gun g) && GameManager.Instance.OwnedGuns.Select(gun => gun.id).Any(id => g.GunSO.id == id))
				{
					// Temp to give 5x ammo at start
					//g.AmmoPouch.AddAmmo(g.GunSO.magSize * 5);
					// Hardcoded as 15 as per design doc
					g.AmmoPouch.AddAmmo(15);
					// Setup the pool
					g.AmmoPouch.SetupPool(g.GunSO.magSize + 1, g.GunSO.bulletPrefab);
					// Works as an instant reload
					g.AmmoPouch.LoadGun(g.GunSO.magSize);
				}
				e.Unequip();
			}
		}
		SelectItem(EquippedItemIndex);
	}

	public void SelectItem(int index)
	{
		if (index >= usableEquipment.Count) return;

		IEquippable itemToEquip = usableEquipment[index];

		if (EquippedItem?.ID == itemToEquip.ID) return;

		EquippedItem?.Unequip();
		EquippedItem = itemToEquip;
		EquippedItem.Equip();
		SoundAlerter.MakeSound(EquippedItem.EquipSound, transform.position);
	}

	// function which gives the player ammo
	public void GiveAmmo(int number)
	{
		if (EquippedItem is Gun gun)
		{
			gun.AmmoPouch.AddAmmo(number);
			HuntingUIManager.Instance.SetAmmoCounterText(gun.AmmoInfo);
		}
	}
}
