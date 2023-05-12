public class ShopFurnitureItem : ItemThumbnailUI
{
    public void SelectItem()
    {
        ShopUIManager.Instance.SelectItem((furnitureSO, inventoryItem));
    }
}
