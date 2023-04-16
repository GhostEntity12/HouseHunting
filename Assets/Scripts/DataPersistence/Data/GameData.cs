using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<(string, int)> serializedInventory;
    public List<SerializableDecoration> serializedDecorations;

    //the values in this constructor will be the initial values of the game state, i.e., when where is no save file
    public GameData()
    {
        serializedInventory = new List<(string, int)>();
        serializedDecorations = new List<SerializableDecoration>();
    }
}
