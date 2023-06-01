using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item")]
public class ShopItemSO : ScriptableObject
{
    public string id;
    public int price;
    public int maxQuantity;
    public Sprite icon;
}
