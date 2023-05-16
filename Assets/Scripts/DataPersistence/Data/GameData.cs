using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<InventoryItem> permanentInventory;
    public List<InventoryItem> huntingInventory;
    public List<HouseItem> houseItems;
    public int currency;

    //the values in this constructor will be the initial values of the game state, i.e., when where is no save file
    public GameData()
    {
        permanentInventory = new List<InventoryItem>();
        huntingInventory = new List<InventoryItem>();
        houseItems = new List<HouseItem>();
        currency = 0;
    }
}
