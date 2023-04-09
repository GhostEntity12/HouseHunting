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
        //after spawning the placeable, select it
        DecorateInputManager.Instance.SelectPlacable(spawned);
        //remove the item from the inventory
        GameManager.Instance.Inventory.RemoveItem(placeableSO);
        //change the quantity of the item in the inventory
        if (int.Parse(quantityText.text) > 1)
            ChangeQuantity(int.Parse(quantityText.text) - 1);
        else
            Destroy(gameObject);
    }
}
