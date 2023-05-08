using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private ItemThumbnailUI itemThumbnailUIPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup tabGroup;
    [SerializeField] private GameObject furnitureDetailsPanel;
    [SerializeField] private ItemThumbnailUI shopTabPrefab;
    [SerializeField] private TextMeshProUGUI noItemsText;

    private (FurnitureSO so, InventoryItem inventoryItem)? selectedFurniture;
    private List<string> tabs;
    private List<InventoryItem> currentDisplayedItems;

    public bool IsShopOpen => shopCanvas.enabled;

    protected override void Awake()
    {
        base.Awake();
        shopCanvas.enabled = false;

        SelectItem(null);
        tabs = new List<string>();
        currentDisplayedItems = new List<InventoryItem>();
    }

    private void Start()
    {
        foreach (InventoryItem item in GameManager.Instance.PermanentInventory.Items)
        {
            // add a tab for this item if it doesn't exist
            if (!tabs.Contains(item.id))
            {
                tabs.Add(item.id);
                ItemThumbnailUI shopTab = Instantiate(shopTabPrefab, tabGroup.transform);
                shopTab.SetItem(item);
                shopTab.SetTab();
            }
        }

        tabs.Sort();

        if (tabs.Count > 0)
            SetTab(tabs[0]);

        RepaintShop();
    }

    private void RepaintShop()
    {
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
            foreach (InventoryItem item in currentDisplayedItems)
            {
                var itemThumbnailUI = Instantiate(itemThumbnailUIPrefab, gridLayoutGroup.transform);
                itemThumbnailUI.SetItem(item);
            }
        }
    }

    public void ToggleShop()
    {
        shopCanvas.enabled = !shopCanvas.enabled;
        if (IsShopOpen)
        {
            Debug.Log(GameManager.Instance.Currency);
            GameManager.Instance.ShowCursor();
            RepaintShop();
        }
        else
        {
            SelectItem(null);
            GameManager.Instance.HideCursor();
        }
    }

    public void SelectItem((FurnitureSO so, InventoryItem inventoryItem)? selectedInventoryItem)
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
        currentDisplayedItems = GameManager.Instance.PermanentInventory.Items.FindAll(x => x.id == tabName);
        RepaintShop();
        SelectItem(null);
    }

    public void SellItem()
    {
        if (selectedFurniture is null) return;

        GameManager.Instance.Currency += (int)selectedFurniture?.inventoryItem.price;
        GameManager.Instance.PermanentInventory.RemoveItem((InventoryItem)selectedFurniture?.inventoryItem);
        SetTab(selectedFurniture?.inventoryItem.id);
        SelectItem(null);

        Debug.Log(GameManager.Instance.Currency);
    }
}