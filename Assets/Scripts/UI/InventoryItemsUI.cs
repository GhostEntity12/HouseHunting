using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemsUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI quantityText;

    private PlaceableSO placeableSO;

    public void SetPlaceablePrefab(PlaceableSO placebleSO, int quantity)
    {
        this.placeableSO = placebleSO;
        image.sprite = placebleSO.thumbnail;
        quantityText.text = quantity.ToString();
    }

    public void ChangeQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
    }

    public void SpawnPlaceable()
    {
        Placeable spawned = Instantiate(placeableSO.placeablePrefab);
        GameManager.Instance.Inventory.RemoveItem(placeableSO);
        if (int.Parse(quantityText.text) > 1)
            ChangeQuantity(int.Parse(quantityText.text) - 1);
        else
            Destroy(gameObject);
    }
}
