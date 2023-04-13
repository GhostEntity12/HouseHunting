[System.Serializable]
public class GameData
{
    //test data
    public int number;

    //the values in this constructor will be the initial values of the game state, i.e., when where is no save file
    public GameData()
    {
        number = 69;
    }
}
