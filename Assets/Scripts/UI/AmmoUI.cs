using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI magazineLeftText;
    [SerializeField] private HorizontalLayoutGroup bulletIconsLayoutGroup;
    [SerializeField] private AmmoIcon ammoIconPrefab;

    private readonly List<AmmoIcon> bulletIcons = new();
    
    // reload animation stuff
    private float reloadTimer = 0f;
    [NonSerialized] public bool isReloading = false;
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
            foreach (AmmoIcon icon in bulletIcons)
            {
                // only trigger animations on icons that are not filled
                if (icon.fillAmount != 1)
                    icon.SetFill(progress);
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

            bulletIcons.Add(icon);
        }
        SetBulletsLeft(newGun.NumberOfShotsLeft);
        MagazineLeft = newGun.NumberOfMagazineLeft;
        bulletIconsLayoutGroup.spacing = newGun.GunSO.uiAmmoSpacing;

	}

    public void SetBulletsLeft(int bulletsLeft)
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            bulletIcons[bulletIcons.Count - i - 1].SetFill(i >= bulletsLeft ? 0 : 1);
        }
    }
}
