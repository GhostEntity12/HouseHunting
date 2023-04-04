using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemsUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI quantityText;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlaceable(PlaceableSO placeableSO, int quantity)
    {
        image.sprite = placeableSO.thumbnail;
        quantityText.text = quantity.ToString();
    }

    public void ChangeQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();
    }
}
