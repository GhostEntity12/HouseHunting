using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryItemsUI inventoryItemsUIPrefab;

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
        foreach (InventoryItem item in GameManager.Instance.PermanentInventory.Items)
        {
            InventoryItemsUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetPlaceablePrefab(item);
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
            InventoryItemsUI inventoryItem = Instantiate(inventoryItemsUIPrefab, transform);
            inventoryItem.SetPlaceablePrefab(item);
        }
    }
}
