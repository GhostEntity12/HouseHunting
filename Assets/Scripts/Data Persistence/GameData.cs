using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<SaveDataFurniture> storedFurniture;
    public List<SaveDataFurniture> huntingInventoryFurniture;
    public List<SaveDataPlacedFurniture> placedFurniture;
    public List<SaveDataGun> gunSaveData;
    public int currency;

    //the values in this constructor will be the initial values of the game state, i.e., when there is no save file
    public GameData()
    {
        storedFurniture = new();
        placedFurniture = new();
        huntingInventoryFurniture= new();
        gunSaveData = new();
        currency = 0;
    }
}
