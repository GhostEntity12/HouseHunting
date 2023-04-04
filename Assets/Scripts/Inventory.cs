using System.Collections.Generic;

public class Inventory
{
    private List<(Placeable furniture, int quantity)> items;

    public List<(Placeable furniture, int quantity)> Items => items;

    public Inventory()
    {
        items = new List<(Placeable furniture, int quantity)>();
    }

    public void AddItem(Placeable furniture)
    {
        Placeable item = items.Find(x => x.furniture == furniture).furniture;

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
