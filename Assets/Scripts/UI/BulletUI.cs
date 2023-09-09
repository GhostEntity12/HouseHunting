using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI magazineLeftText;
    [SerializeField] private HorizontalLayoutGroup bulletIconsLayoutGroup;

    private List<Image> bulletIcons = new List<Image>();

    public void ChangeGun(Gun newGun)
    {
        foreach (Image icon in bulletIcons)
            Destroy(icon.gameObject);

        bulletIcons.Clear();

        int numberOfIcons = newGun.AmmoPouch.AmmoInGun / newGun.GunSO.bulletsPerTap;
        for (int i = 0; i < numberOfIcons; i++)
        {
            Image newIcon = new GameObject("BulletIcon").AddComponent<Image>();
            newIcon.sprite = newGun.GunSO.bulletPrefab.Sprite;
            newIcon.transform.parent = bulletIconsLayoutGroup.transform;
            bulletIcons.Add(newIcon);
        }
    }

    public void SetBulletsLeft(int bulletsLeft)
    {
        foreach (Image icon in bulletIcons)
            icon.fillAmount = 0;

        for (int i = 0; i < bulletsLeft; i++)
        {
            bulletIcons[i].fillAmount = 1;
        }
    }
}
