using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
	private readonly List<Gun> allGuns = new();
	private readonly List<Gun> ownedGuns = new();
	private SoundAlerter soundAlerter;

	public int CurrentGunIndex { get; private set; } = 0;
	public Gun CurrentGun { get; private set; } = null;

	private void Start()
	{
		// Return if not in Hunting scene
		if (HuntingManager.Instance == null) return;

		soundAlerter = GameObject.Find("Player").GetComponent<SoundAlerter>();
		// TODO: Swap to this once player is exposed
		//soundAlerter = HuntingManager.Instance.Player.GetComponent<SoundAlerter>();

		foreach (var item in GameManager.Instance.OwnedGuns.Select(g => g.id))
		{
			Debug.Log(item);
		}

		// Iterating over children
		foreach (Transform t in transform)
		{
			if (t.TryGetComponent(out Gun g))
			{
				allGuns.Add(g);
				Debug.Log(g.GunSO.id);
				if (GameManager.Instance.OwnedGuns.Select(g => g.id).Any(i => g.GunSO.id == i))
				{
					Debug.Log($" Adding {g.GunSO.id}");
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
		if (selectedGun != null && selectedGun != CurrentGun)
		{
			if (CurrentGun)
				CurrentGun.gameObject.SetActive(false);
			selectedGun.gameObject.SetActive(true);
			CurrentGun = selectedGun;
			soundAlerter.MakeSound(10, transform.position);
		}

		HuntingUIManager.Instance.SetAmmoCounterText(CurrentGun.AmmoInfo);
	}

	// debug function which gives the player ammo
	public void GiveAmmo(int number)
	{
		CurrentGun.AmmoPouch.AddAmmo(number);
		HuntingUIManager.Instance.SetAmmoCounterText(CurrentGun.AmmoInfo);
	}
}
