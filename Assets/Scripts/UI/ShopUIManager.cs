using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private ShopFurnitureItem shopFurnitureItemPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup tabGroup;
    [SerializeField] private GameObject furnitureDetailsPanel;
    [SerializeField] private ShopTabItem shopTabItemPrefab;
    [SerializeField] private TextMeshProUGUI noItemsText;
    [SerializeField] private TextMeshProUGUI currencyText;

    private (FurnitureSO so, FurnitureItem inventoryItem)? selectedFurniture;
    private List<string> tabs;
    private List<FurnitureItem> currentDisplayedItems;
    private Inventory inventory;

    public bool IsShopOpen => shopCanvas.enabled;
    public (FurnitureSO so, FurnitureItem inventoryItem)? SelectedFurniture => selectedFurniture;

    protected override void Awake()
    {
        base.Awake();
        shopCanvas.enabled = false;

        SelectItem(null);
        tabs = new List<string>();
        currentDisplayedItems = new List<FurnitureItem>();
    }

    private void Start()
    {
        inventory = HuntingManager.Instance != null ? HuntingManager.Instance.HuntingInventory : GameManager.Instance.PermanentInventory;
    }

    private void RepaintShop()
    {
        noItemsText.gameObject.SetActive(false);

        foreach (Transform child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        if (currentDisplayedItems.Count == 0)
        {
            noItemsText.gameObject.SetActive(true);
        }
        else
        {
            noItemsText.gameObject.SetActive(false);
            foreach (FurnitureItem item in currentDisplayedItems)
            {
                ShopFurnitureItem itemThumbnailUI = Instantiate(shopFurnitureItemPrefab, gridLayoutGroup.transform);
                itemThumbnailUI.SetItem(item);
            }
        }

        if (currencyText != null)
            currencyText.text = "$" + GameManager.Instance.Currency.ToString();
    }

    private void RepaintTab()
    {
        foreach (Transform child in tabGroup.transform)
        {
            Destroy(child.gameObject);
        }

        tabs.Clear();

        foreach (FurnitureItem item in inventory.Items)
        {
            // add a tab for this item if it doesn't exist
            if (!tabs.Contains(item.id))
            {
                tabs.Add(item.id);
            }
        }

        tabs.Sort();

        foreach (string tab in tabs)
        {
            ShopTabItem shopTab = Instantiate(shopTabItemPrefab, tabGroup.transform);
            shopTab.SetItem(inventory.Items.Find(x => x.id == tab));
        }

        if (tabs.Count > 0)
            SetTab(tabs[0]);
        else
            currentDisplayedItems.Clear();

        RepaintShop();
    }

    public void ToggleShop()
    {
        shopCanvas.enabled = !shopCanvas.enabled;
        if (IsShopOpen)
        {
            GameManager.Instance.ShowCursor();
            RepaintTab();
        }
        else
        {
            SelectItem(null);
            GameManager.Instance.HideCursor();
        }
    }

    public void SelectItem((FurnitureSO so, FurnitureItem inventoryItem)? selectedInventoryItem)
    {
        if (selectedInventoryItem is null)
        {
            furnitureDetailsPanel.SetActive(false);
            return;
        }
        selectedFurniture = selectedInventoryItem;
        furnitureDetailsPanel.SetActive(true);
        furnitureDetailsPanel.GetComponent<FurnitureDetailsUI>().SetFurniture(selectedInventoryItem.Value);
    }

    public void SetTab(string tabName)
    {
        currentDisplayedItems = inventory.Items.FindAll(x => x.id == tabName);
        BuyMenuUIManager.Instance.ToggleBuyMenu(false);
        SelectItem(null);
        RepaintShop();
    }

    public void SellItem()
    {
        if (selectedFurniture is null) return;

        GameManager.Instance.Currency += (int)selectedFurniture?.inventoryItem.price;
        inventory.RemoveItem((FurnitureItem)selectedFurniture?.inventoryItem);
        SetTab(selectedFurniture?.inventoryItem.id);
        RepaintTab();
        SelectItem(null);
        InventoryUIManager.Instance.RepaintInventory();
    }

    public void DropItem()
    {
        if (selectedFurniture is null) return;

        inventory.RemoveItem((FurnitureItem)selectedFurniture?.inventoryItem);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // i wanted to use HuntingInputManager. which is attached to the player but it's null for some reason
        Shootable shootable = Instantiate(selectedFurniture?.so.shootablePrefab, player.transform.position + player.transform.forward * 2, Quaternion.identity);
        shootable.Die();
        SetTab(selectedFurniture?.inventoryItem.id);
        RepaintTab();
        SelectItem(null);
        // uncomment this if you want the shop to close after dropping an item, its better to keep it open if you want to drop multiple items
        // otherwise its better to close it if the player is only dropping one item.
        // ToggleShop();
    }
}
