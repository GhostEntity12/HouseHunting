using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Inventory
{
    private List<FurnitureItem> furnitures;
    private List<ShopItem> boughtItems = new List<ShopItem>();

	public List<FurnitureItem> Items { get { return furnitures; } set { furnitures = value; } }
    public List<ShopItem> BoughtItems { get { return boughtItems; } set { boughtItems = value; } }

    public Inventory()
    {
        furnitures = new List<FurnitureItem>();
        boughtItems = new List<ShopItem>();
    }

    public void AddItem(FurnitureItem newItem)
    {
        furnitures.Add(newItem);
    }

    public void RemoveItem(FurnitureItem itemToRemove)
    {
        furnitures.Remove(itemToRemove);
    }

    public void MergeInventory(Inventory other)
    {
        furnitures.AddRange(other.Items);

        other.ClearInventory();
    }

    public void ClearInventory()
    {
        furnitures.Clear();
    }

    public void SetBuyableQuantity(string id, int quantity)
    {
        foreach (ShopItem item in boughtItems)
        {
            if (item.id == id)
            {
                item.quantity = quantity;
                return;
            }
        }
    }

    public override string ToString()
    {
        if (furnitures.Count == 0)
            return "Empty inventory";

        string result = "";

        foreach (var item in furnitures)
        {
            result += $"{item.id}, {item.scaleFactor}, {item.materialIndex}, {item.price}\n";
        }

        return result;
    }

    public void PrintBoughtItems()
    {
        UnityEngine.Debug.Log(string.Join(", ", BoughtItems.Select(item => item.id)));
    }
}
