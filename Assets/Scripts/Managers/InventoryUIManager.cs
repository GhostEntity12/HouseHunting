using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private ItemThumbnailUI inventoryItemsUIPrefab;

    private static InventoryUIManager instance;

    public static InventoryUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        foreach (FurnitureItem item in GameManager.Instance.PermanentInventory.Items)
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
        foreach (FurnitureItem item in GameManager.Instance.PermanentInventory.Items)
        {
            ItemThumbnailUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetItem(item);
        }
    }
}
