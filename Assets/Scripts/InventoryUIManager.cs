using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryItemsUI inventoryItemsUIPrefab;
    private static InventoryUIManager instance;

    public static InventoryUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        foreach (var item in GameManager.Instance.Inventory.Items)
        {
            InventoryItemsUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetPlaceablePrefab(item.furniture, item.quantity);
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
        foreach (var item in GameManager.Instance.Inventory.Items)
        {
            InventoryItemsUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetPlaceablePrefab(item.furniture, item.quantity);
        }
    }
}
