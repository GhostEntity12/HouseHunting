using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemsUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI quantityText;

    private Placeable placeablePrefab;

    public void SetPlaceablePrefab(Placeable placeablePrefab, int quantity)
    {
        this.placeablePrefab = placeablePrefab;
        image.sprite = placeablePrefab.PlaceableSO.thumbnail;
        quantityText.text = quantity.ToString();
    }

    public void ChangeQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
    }

    public void SpawnPlaceable()
    {
        Placeable spawned = Instantiate(placeablePrefab);
        GameManager.Instance.Inventory.RemoveItem(placeablePrefab);
        if (int.Parse(quantityText.text) > 1)
            ChangeQuantity(int.Parse(quantityText.text) - 1);
        else
            Destroy(gameObject);
    }
}
