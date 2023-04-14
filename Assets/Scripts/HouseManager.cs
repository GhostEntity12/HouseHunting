using UnityEngine;

public class HouseManager : MonoBehaviour, IDataPersistence
{
    private static HouseManager instance;
    private int number;

    public static HouseManager Instance { get; private set; }

    public void LoadData(GameData data)
    {
        this.number = data.number;
    }

    public void SaveData(GameData data)
    {
    }

    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    private void Start() 
    {
        Debug.Log(number);
    }
}
