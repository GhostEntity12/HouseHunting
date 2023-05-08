using UnityEngine;
using UnityEngine.UI;

public class ItemThumbnailUI : MonoBehaviour
{
    [SerializeField] private Image image;

    private InventoryItem inventoryItem;
    private FurnitureSO furnitureSO;

    public void SetItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        furnitureSO = DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == inventoryItem.id);
        image.sprite = furnitureSO.thumbnail;
    }

    public void SpawnPlaceable()
    {
        Placeable spawned = Instantiate(furnitureSO.placeablePrefab);
        //after spawning the placeable, select it
        HouseInputManager.Instance.SelectPlacable(spawned);
        //remove the item from the inventory
        GameManager.Instance.PermanentInventory.RemoveItem(inventoryItem);
        //repaint the inventory
        InventoryUIManager.Instance.RepaintInventory();

        spawned.InventoryItem = inventoryItem;
    }

    public void SelectItem()
    {
        ShopUIManager.Instance.SelectItem((furnitureSO, inventoryItem));
    }

    public void SetTab()
    {
        ShopUIManager.Instance.SetTab(furnitureSO.id);
    }
}
