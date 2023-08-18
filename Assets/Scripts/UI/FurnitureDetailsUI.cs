using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureDetailsUI : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scaleFactorText;
    [SerializeField] private TextMeshProUGUI materialText;
    [SerializeField] private TextMeshProUGUI priceText;

    public void SetFurniture((FurnitureSO so, SaveDataFurniture inventoryItem) furniture)
    {
        thumbnail.sprite = furniture.so.thumbnail;
        nameText.text = furniture.so.name;
        scaleFactorText.text = furniture.inventoryItem.scaleFactor.ToString();
        materialText.text = furniture.so.materials[furniture.inventoryItem.materialIndex].name ?? "None";
        priceText.text = furniture.inventoryItem.price.ToString();
    }
}
