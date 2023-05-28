using UnityEngine;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    [SerializeField] private ItemThumbnailUI inventoryItemsUIPrefab;

    private void Start()
    {
        foreach (InventoryItem item in GameManager.Instance.PermanentInventory.Items)
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
        foreach (InventoryItem item in GameManager.Instance.PermanentInventory.Items)
        {
            ItemThumbnailUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetItem(item);
        }
    }
}
