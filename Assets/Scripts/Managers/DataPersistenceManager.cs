using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    [SerializeField] private List<FurnitureSO> allFurnitureSOs; // this list stores all the placeable scriptable objects in the game, every time a new one is created, it must be added to this list via the Unity editor
    [SerializeField] private List<ShopItemSO> allShopItems; // this list stores all the buyable items in the game, every time a new one is created, it must be added to this list via the Unity editor
    [SerializeField] private string savedFileName = "data";

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler fileDataHandler;

    public List<FurnitureSO> AllFurnitureSO { get => allFurnitureSOs; }
    public List<ShopItemSO> AllShopItems { get => allShopItems; }

    protected override void Awake() 
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, savedFileName);
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void NewGame()
    {
        gameData = new GameData();
        gameData.boughtItems.Add(new GunShopItem("crossbow", 1, 1000));
        gameData.boughtItems.Add(new GunShopItem("rifle", 1, 1000));
        gameData.boughtItems.Add(new GunShopItem("shotgun", 1, 1000));
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.Load();

        if (gameData == null)
            NewGame();

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(gameData);
        }

        fileDataHandler.Save(gameData);
    }

    public Placeable GetPlaceablePrefabById(string id)
    {
        return allFurnitureSOs.Find(x => x.id == id).placeablePrefab;
    }

    public ShopItemSO GetShopItemById(string id)
    {
        Debug.Log($"Seraching for {id}");
        Debug.Log($"Search size: {allShopItems.Count}");
        foreach (var item in allFurnitureSOs)
        {
            Debug.Log(item);
            Debug.Log(item.id);
        }

        Debug.Log($"Search options: {string.Join(", ", allShopItems.Select(shopItem => shopItem.id))}");

        return allShopItems.Find(x => x.id == id);
    }
}
