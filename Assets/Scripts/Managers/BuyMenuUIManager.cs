using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyMenuUIManager : Singleton<BuyMenuUIManager>
{
    [SerializeField] private BuyMenuItemUI buyMenuItemPrefab;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private ShopItemDetailsUI shopItemDetailsPanel;

    private ShopItemSO selectedShopItem;

    protected override void Awake()
    {
        base.Awake();
        shopItemDetailsPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.LogError("Tried to open the shop - currently disabled.", this);
        return;
        //if (DataPersistenceManager.Instance != null)
        //{
        //    foreach (ShopItemSO item in DataPersistenceManager.Instance.AllShopItems)
        //    {
        //        BuyMenuItemUI buyMenuItem = Instantiate(buyMenuItemPrefab, verticalLayoutGroup.transform);
        //        buyMenuItem.SetItem(item);
        //    }
        //}
    }

    private void OnDisable()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        shopItemDetailsPanel.gameObject.SetActive(false);
    }

    public void ToggleBuyMenu(bool open)
    {
        if (open)
        {
            gameObject.SetActive(true);
            ShopUIManager.Instance.SelectItem(null);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SelectItem(ShopItemSO item)
    {
        shopItemDetailsPanel.gameObject.SetActive(item != null);
        shopItemDetailsPanel.SetItem(item);
        selectedShopItem = item;
    }

    public void BuySelectedItem()
    {
		Debug.LogError("Tried to purchase and item from the shop - currently disabled.", this);
		return;
		//if (selectedShopItem != null)
        //{
        //    // check if the player has enough money
        //    if (GameManager.Instance.Currency < selectedShopItem.price) return;
        //    GameManager.Instance.Currency -= selectedShopItem.price;
        //
        //    ShopItem itemToBuy = GameManager.Instance.PermanentInventory.BoughtItems.Find(item => item.id == selectedShopItem.id);
        //    if (itemToBuy == null)
        //    {
        //        itemToBuy = new ShopItem(selectedShopItem.id, 1);
        //        GameManager.Instance.PermanentInventory.BoughtItems.Add(itemToBuy);
        //    }
        //    else
        //    {
        //        itemToBuy.quantity++;
        //    }
        //
        //    // refresh the UI
        //    shopItemDetailsPanel.SetItem(selectedShopItem);
        //}
    }
}
