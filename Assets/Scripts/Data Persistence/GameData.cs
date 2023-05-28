using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<InventoryItem> permanentInventory;
    public List<InventoryItem> huntingInventory;
    public List<HouseItem> houseItems;
    public List<WeaponInventoryItem> gunAmmo;
    public int currency;

    //the values in this constructor will be the initial values of the game state, i.e., when there is no save file
    public GameData()
    {
        permanentInventory = new List<InventoryItem>();
        huntingInventory = new List<InventoryItem>();
        houseItems = new List<HouseItem>();
        gunAmmo = new List<WeaponInventoryItem>();
        currency = 0;
    }
}
