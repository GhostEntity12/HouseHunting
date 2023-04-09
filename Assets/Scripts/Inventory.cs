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
        PlaceableSO item = items.Find(x => x.furniture == furniture).furniture;

        if (item != null)
        {
            int index = items.FindIndex(x => x.furniture == furniture);
            if (items[index].quantity > 1)
                items[index] = (item, items[index].quantity - 1);
            else
                items.RemoveAt(index);
        }
    }
}
