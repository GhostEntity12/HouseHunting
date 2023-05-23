using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyMenuItemUI : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameText;

    private ShopItemSO shopItem;

    public void SetItem(ShopItemSO item)
    {
        shopItem = item;
        thumbnail.sprite = item.icon;
        nameText.text = item.name;
    }
}
