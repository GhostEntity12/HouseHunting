using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private ItemThumbnailUI itemThumbnailUIPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject furnitureDetailsPanel;

    private (FurnitureSO so, InventoryItem inventoryItem) selectedFurniture;

    public bool IsShopOpen => shopCanvas.enabled;

    protected override void Awake()
    {
        base.Awake();
        shopCanvas.enabled = false;

        SelectFurniture(null);
    }

    private void Start()
    {
        foreach (InventoryItem item in GameManager.Instance.PermanentInventory.Items)
        {
            var itemThumbnailUI = Instantiate(itemThumbnailUIPrefab, gridLayoutGroup.transform);
            itemThumbnailUI.SetItem(item);
        }
    }

    public void ToggleShop()
    {
        shopCanvas.enabled = !shopCanvas.enabled;
        if (IsShopOpen)
            GameManager.Instance.ShowCursor();
        else
        {
            SelectFurniture(null);
            GameManager.Instance.HideCursor();
        }
    }

    public void SelectFurniture((FurnitureSO so, InventoryItem inventoryItem)? selectedInventoryItem)
    {
        if (selectedInventoryItem is null)
        {
            furnitureDetailsPanel.SetActive(false);
            return;
        }
        furnitureDetailsPanel.SetActive(true);
        furnitureDetailsPanel.GetComponent<FurnitureDetailsUI>().SetFurniture(selectedInventoryItem.Value);
    }
}
