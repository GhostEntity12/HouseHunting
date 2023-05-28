using System.Collections.Generic;

public class Inventory
{
    private List<InventoryItem> items;
    private List<WeaponInventoryItem> gunAmmo = new List<WeaponInventoryItem>();

	public List<InventoryItem> Items 
    { 
        get { return items; } 
        set { items = value; } 
    }
    public List<WeaponInventoryItem> GunAmmo { get; set; }

    public Inventory()
    {
        items = new List<InventoryItem>();
        gunAmmo = new List<WeaponInventoryItem>();
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

    public void AddGun(string id)
    {
        gunAmmo.Add(new WeaponInventoryItem(id));
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
