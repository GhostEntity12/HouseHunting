using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponWheel : MonoBehaviour
{
    [SerializeField] private Image weaponWheelItemPrefab;
    [SerializeField] private Image insideWheel;

    private List<Image> weaponWheelItems = new List<Image>();
    private int selectedIndex = 0;

    private int DistinctItemCount => GameManager.Instance.PermanentInventory.BoughtItems.Where(x => x is GunShopItem).Cast<GunShopItem>().ToList().Count;

    private void Start()
    {
        float gapAngle = DistinctItemCount > 1 ? 2f : 0f;
        float segmentAngle = 360f / DistinctItemCount; // the angle of each segment of the wheel, i.e., how much its takes up of the circle

        for (int i = 0; i < DistinctItemCount; i++)
        {
            // instantiate weapon wheel item (full circle)
            Image item = Instantiate(weaponWheelItemPrefab, transform);
            // adjust the fill amount of the wheel to the size of the segment, but subtract half the gap angle
            item.fillAmount = 1f / DistinctItemCount - gapAngle / 2 / 360f;

            item.transform.localPosition = Vector3.zero;
            // rotate the wheel item by the angle of the segment
            item.transform.rotation = Quaternion.Euler(0f, 0f, -segmentAngle * i - gapAngle / 2);

            // add the weapon wheel item to the list to keep track of it
            weaponWheelItems.Add(item);
            
            // instantiate image icon for each weapon
            // this wheel is 65% of the size of the weapon wheel item, which the weapon icon lies on
            float iconWheelRadius = item.rectTransform.rect.width / 2 * 0.65f;

            // create a new game object and add an image component to it
            Image icon = new GameObject("Icon").AddComponent<Image>();
            icon.transform.SetParent(transform);
            // set the icon's sprite to the weapon's icon

            GameManager.Instance.PermanentInventory.PrintBoughtItems();

            Debug.Log("SPRITE:");
			icon.sprite = DataPersistenceManager.Instance.GetShopItemById(GameManager.Instance.PermanentInventory.BoughtItems[i].id).icon;

            Debug.Log(icon.sprite);

            // calculate the angle of the icon based on the index of the weapon, i.e., which segment of the wheel it is in
            float angleInDegrees = 360 / DistinctItemCount * i;

            // offset angle by half angle to center the icon in the segment
            angleInDegrees += 360 / DistinctItemCount / 2;
            // convert the angle to radians
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            // calculate the x and y coordinates of the icon based on the angle
            float x = iconWheelRadius * Mathf.Sin(angleInRadians);
            float y = iconWheelRadius * Mathf.Cos(angleInRadians);

            // set the position and size of the icon
            icon.transform.localPosition = new Vector3(x, y, 0f);
            icon.rectTransform.sizeDelta = new Vector2(100f, 100f);
        }

        insideWheel.fillAmount = 1f / DistinctItemCount;

        CloseWeaponWheel();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            // calculate the angle of the mouse from the center of the screen
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            
            float distanceBetweenMouseAndCenter = Vector2.Distance(mousePos, center);
            if (distanceBetweenMouseAndCenter < insideWheel.rectTransform.rect.width / 2)
            {
                weaponWheelItems.ForEach(item => item.color = Color.gray);
                selectedIndex = WeaponManager.Instance.CurrentGunIndex;
                return;
            }

            // calculate the angle of the mouse from the center of the screen
            float mouseAngleFromCenter = Vector2.SignedAngle(mousePos - center, Vector2.up); // returns -180 to 180
            // convert the angle to a value between 0 and 360
            mouseAngleFromCenter = (mouseAngleFromCenter + 360) % 360;

            // calculate the angle of each segment of the wheel
            float anglePerSegment = 360f / DistinctItemCount;
            selectedIndex = Mathf.FloorToInt(mouseAngleFromCenter / anglePerSegment);

            // highlight the selected item
            weaponWheelItems.ForEach(item => item.color = Color.gray);
            weaponWheelItems[selectedIndex].color = Color.white;

            // rotate the inside wheel to follow the mouse
            insideWheel.transform.rotation = Quaternion.Euler(0f, 0f, -mouseAngleFromCenter + anglePerSegment / 2);
        }
    }

    public void OpenWeaponWheel()
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseWeaponWheel()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        WeaponManager.Instance.SelectItem(selectedIndex);
    }
}
