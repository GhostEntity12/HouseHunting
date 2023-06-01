using UnityEngine;
using UnityEngine.UI;

public class ItemThumbnailUI : MonoBehaviour
{
    [SerializeField] private Image image;

    protected FurnitureItem inventoryItem;
    protected FurnitureSO furnitureSO;

    public void SetItem(FurnitureItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        furnitureSO = DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == inventoryItem.id);
        image.sprite = furnitureSO.thumbnail;
    }
}
