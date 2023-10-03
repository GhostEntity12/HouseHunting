using System.Collections.Generic;
using UnityEngine;

public class FurnitureInventory
{
	private const int maxInventorySize = 1000; // Set the maximum inventory size to 10

	public List<SaveDataFurniture> Furniture { get; set; }

	public FurnitureInventory()
    {
        Furniture = new();
    }

    public void AddItem(SaveDataFurniture newItem)
    {
        if (Furniture.Count < maxInventorySize)
        {
            Furniture.Add(newItem);
        }
        else
        {
            // Handle the case when the inventory is already full
            // You can throw an exception, display an error message, or simply ignore the addition.
            // For this example, we'll ignore the addition.
            Debug.Log("Inventory is already full. Cannot add more items.");
        }
    }
	public void MergeInventory(FurnitureInventory other)
	{
		Furniture.AddRange(other.Furniture);
		other.ClearInventory();
	}

	public void RemoveItem(SaveDataFurniture itemToRemove) => Furniture.Remove(itemToRemove);

	public void ClearInventory() => Furniture.Clear();

    public override string ToString()
    {
        if (Furniture.Count == 0)
            return "Empty inventory";

        string result = "";

        foreach (var item in Furniture)
        {
            result += $"{item.id}";
        }

        return result;
    }
}
