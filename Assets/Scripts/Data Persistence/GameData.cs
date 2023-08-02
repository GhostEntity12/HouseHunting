using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<FurnitureItem> permanentInventory;
    public List<FurnitureItem> huntingInventory;
    public List<HouseItem> houseItems;
    public List<ShopItem> boughtItems;
    public int currency;

    //the values in this constructor will be the initial values of the game state, i.e., when there is no save file
    public GameData()
    {
        permanentInventory = new List<FurnitureItem>();
        huntingInventory = new List<FurnitureItem>();
        houseItems = new List<HouseItem>();
        boughtItems = new List<ShopItem>();
        currency = 0;
    }
}
