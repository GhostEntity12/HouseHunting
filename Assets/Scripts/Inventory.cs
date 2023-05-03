using System.Collections.Generic;

public class Inventory
{
    private List<InventoryItem> items;

    public List<InventoryItem> Items => items;

    public Inventory()
    {
        items = new List<InventoryItem>();
    }

    public void AddItem(InventoryItem newItem)
    {
        items.Add(newItem);
    }

    public void RemoveItem(InventoryItem itemToRemove)
    {
        items.Remove(itemToRemove);
    }

    public void MergeInventory(Inventory other)
    {
        items.AddRange(other.Items);

        other.ClearInventory();
    }

    public void ClearInventory()
    {
        items.Clear();
    }

    public void SetInventory(List<InventoryItem> newItems)
    {
        items = newItems;
    }

    public override string ToString()
    {
        if (items.Count == 0)
            return "Empty inventory";

        string result = "";

        foreach (var item in items)
        {
            result += $"{item.id}, {item.scaleFactor}, {item.materialIndex}, {item.price}\n";
        }

        return result;
    }
}
