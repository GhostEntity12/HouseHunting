public class DecorateListItem : ItemThumbnailUI
{
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
}
