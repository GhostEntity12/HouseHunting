using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    //test data
    public int number;

    public List<(string, int)> serializedInventory;

    //the values in this constructor will be the initial values of the game state, i.e., when where is no save file
    public GameData()
    {
        number = 0;
        serializedInventory = new List<(string, int)>();
    }
}
