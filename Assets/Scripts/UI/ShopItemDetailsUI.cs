using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetailsUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Button buyButton;

    public void SetItem(ShopItemSO item)
    {
        itemImage.sprite = item.icon;
        itemNameText.text = item.name;

        ShopItem itemInInventory = GameManager.Instance.PermanentInventory.BoughtItems.Find(x => x.id == item.id);
        int quantity = itemInInventory != null ? itemInInventory.quantity : 0;
        itemDescriptionText.text = $"Price: ${item.price}\n{quantity}/{item.maxQuantity}";

        buyButton.interactable = quantity < item.maxQuantity && GameManager.Instance.Currency >= item.price;
    }
}
