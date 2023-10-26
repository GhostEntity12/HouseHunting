using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI magazineLeftText;
    [SerializeField] private Image magazineImage;
    [SerializeField] private HorizontalLayoutGroup bulletIconsLayoutGroup;
    [SerializeField] private AmmoIcon ammoIconPrefab;

    private readonly List<AmmoIcon> bulletIcons = new();
    
    // reload animation stuff
    private float reloadTimer = 0f;
    [NonSerialized] public bool isReloading = false;
    [NonSerialized] public int bulletsToReload = 0;
    [NonSerialized] public float reloadTime; // Time it takes to reload the gun

    public int MagazineLeft
    {
        set
        {
            magazineLeftText.text = value.ToString();
        }
    }

	public delegate void OnReloadFinish();
	public static event OnReloadFinish OnReloadFinishEvent;

    private void Update()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;

            float progress = reloadTimer / reloadTime;

            // Update the reload bar fill amount
            if (EquipmentManager.Instance.EquippedItem is Gun g) 
            {
                for (int i = bulletIcons.Count - g.AmmoPouch.AmmoInGun - bulletsToReload; i < bulletIcons.Count; i++)
                {
                    AmmoIcon icon = bulletIcons[i];
                    // only trigger animations on icons that are not filled
                    if (icon.FillAmount != 1)
                        icon.SetFill(progress);
                }
            }

            if (reloadTimer >= reloadTime)
            {
                // If the gun has finished reloading
                isReloading = false;
                reloadTimer = 0;
                OnReloadFinishEvent?.Invoke();
            }
        }
    }

    public void Rerender(Gun newGun)
    {
        foreach (AmmoIcon icon in bulletIcons)
            Destroy(icon.gameObject);

        bulletIcons.Clear();

        for (int i = 0; i < newGun.MaxShotPerMagazine; i++)
        {
            AmmoIcon icon = Instantiate(ammoIconPrefab, bulletIconsLayoutGroup.transform);
            icon.Initiate(newGun.GunSO.bulletPrefab.Icon);
            icon.transform.name = "Bullet " + i;

            bulletIcons.Add(icon);
        }
        SetBulletsLeft(newGun.NumberOfShotsLeft);
        MagazineLeft = newGun.NumberOfMagazineLeft;
        bulletIconsLayoutGroup.spacing = newGun.GunSO.uiAmmoSpacing;

        magazineImage.sprite = newGun.GunSO.bulletPrefab.MagazineIcon;
	}

    public void SetBulletsLeft(int bulletsLeft)
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            bulletIcons[bulletIcons.Count - i - 1].SetFill(i >= bulletsLeft ? 0 : 1);
        }
    }
}
