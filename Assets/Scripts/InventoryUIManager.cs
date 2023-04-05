using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryItemsUI inventoryItemsUIPrefab;
    private static InventoryUIManager instance;

    public static InventoryUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
