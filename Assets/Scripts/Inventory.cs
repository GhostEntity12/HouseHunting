using System.Collections.Generic;

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

    public void MergeInventory(Inventory other)
    {
        foreach ((PlaceableSO furniture, int quantity) item in other.items)
        {
            for (int i = 0; i < item.quantity; i++)
            {
                AddItem(item.furniture);
            }
        }

        other.ClearInventory();
    }

    public void ClearInventory()
    {
        items.Clear();
    }

    public List<(string, int)> Serialize()
    {
        List<(string, int)> serializedInventory = new List<(string, int)>();

        foreach ((PlaceableSO furniture, int quantity) item in items)
        {
            serializedInventory.Add((item.furniture.name, item.quantity));
        }

        return serializedInventory;
    }

    public static Inventory Deserialize(List<(string, int)> serializedInventory)
    {
        Inventory inventory = new Inventory();

        foreach ((string furnitureName, int quantity) item in serializedInventory)
        {
            PlaceableSO furniture = DataPersistenceManager.Instance.PlaceableScriptableObjects.Find(x => x.name == item.furnitureName);
            inventory.items.Add((furniture, item.quantity));
        }

        return inventory;
    }

    public override string ToString()
    {
        if (items.Count == 0)
            return "Empty inventory";

        string result = "";

        foreach ((PlaceableSO furniture, int quantity) item in items)
        {
            result += item.furniture.name + " x" + item.quantity + ", ";
        }

        return result;
    }
}
