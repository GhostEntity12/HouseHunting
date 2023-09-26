public class ShopFurnitureItem : ItemThumbnailUI
{
    public void SelectItem()
    {
        InventoryUIManagerOld.Instance.SelectItem((furnitureSO, inventoryItem));
    }
}
