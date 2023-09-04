using UnityEngine;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    [SerializeField] private ItemThumbnailUI inventoryItemsUIPrefab;

    private void Start()
    {
        foreach (SaveDataFurniture item in GameManager.Instance.PermanentInventory.Furniture)
        {
            ItemThumbnailUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetItem(item);
        }
    }

    public void RepaintInventory()
    {
        //remove all items UI from the inventory
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //add all items UI from the inventory
        foreach (SaveDataFurniture item in GameManager.Instance.PermanentInventory.Furniture)
        {
            ItemThumbnailUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetItem(item);
        }
    }
}
