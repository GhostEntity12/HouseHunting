using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<(PlaceableSO furniture, int quantity)> items;

    public List<(PlaceableSO furniture, int quantity)> Items => items;

    public Inventory()
    {
        items = new List<(PlaceableSO furniture, int quantity)>();
    }

    public void AddItem(PlaceableSO furniture)
    {
        Debug.Log($"Adding {furniture.name} to inventory");
        PlaceableSO item = items.Find(x => x.furniture == furniture).furniture;

        if (item != null)
        {
            int index = items.FindIndex(x => x.furniture == furniture);
            items[index] = (item, items[index].quantity + 1);
        }
        else
        {
            items.Add((furniture, 1));
        }
    }

    public void RemoveItem(PlaceableSO furniture)
    {
        Debug.Log($"Removing {furniture.name} from inventory");
        PlaceableSO item = items.Find(x => x.furniture == furniture).furniture;

        if (item != null)
        {
            int index = items.FindIndex(x => x.furniture == furniture);
            if (items[index].quantity > 1)
            {
                items[index] = (item, items[index].quantity - 1);
            }
            else
            {
                items.RemoveAt(index);
            }
        }
    }

    public override string ToString()
    {
        string result = "";
        foreach (var item in items)
        {
            result += $"{item.furniture.name} x{item.quantity}";
        }
        return result == "" ? "Empty Inventory" : result;
    }
}
