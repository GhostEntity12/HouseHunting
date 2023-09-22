using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI magazineLeftText;
    [SerializeField] private HorizontalLayoutGroup bulletIconsLayoutGroup;

    private readonly List<Image> bulletIcons = new List<Image>();
    
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
            foreach (Image image in bulletIcons)
            {
                // only trigger animations on icons that are not filled
                if (image.fillAmount != 1)
                    image.fillAmount = progress;
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
        foreach (Image icon in bulletIcons)
            Destroy(icon.gameObject);

        bulletIcons.Clear();

        for (int i = 0; i < newGun.MaxShotPerMagazine; i++)
        {
            // create new game object and attach image component to it.
            Image newIcon = new GameObject("BulletIcon" + i).AddComponent<Image>();
            newIcon.sprite = newGun.GunSO.bulletPrefab.Sprite;
            newIcon.transform.parent = bulletIconsLayoutGroup.transform;

            // Calculate the size of the child object based on the sprite's dimensions.
            float spriteWidth = newIcon.sprite.rect.width;
            float spriteHeight = newIcon.sprite.rect.height;
            float aspectRatio = spriteWidth / spriteHeight;

            // Set the calculated size to the RectTransform of the child object.
            RectTransform rectTransform = newIcon.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100 * aspectRatio, 100); // You can adjust the 100 as needed.

            // set new icon to type filled
            newIcon.type = Image.Type.Filled;
            newIcon.fillMethod = Image.FillMethod.Vertical;
            newIcon.fillAmount = 1;
            newIcon.preserveAspect = true;

            bulletIcons.Add(newIcon);
        }
        SetBulletsLeft(newGun.NumberOfShotsLeft);
        MagazineLeft = newGun.NumberOfMagazineLeft;
    }

    public void SetBulletsLeft(int bulletsLeft)
    {
        foreach (Image icon in bulletIcons)
            icon.fillAmount = 0;

        for (int i = 0; i < bulletsLeft; i++)
        {
            bulletIcons[bulletIcons.Count - i - 1].fillAmount = 1;
        }
    }
}
