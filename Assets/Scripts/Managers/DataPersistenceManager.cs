using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
	[Tooltip("This list stores all the placeable scriptable objects in the game, every time a new one is created, it must be added to this list via the Unity editor")]
	[SerializeField] private List<FurnitureSO> allFurnitureSOs;
    [SerializeField] private List<GunSO> allGunSOs;
    [Tooltip("This list stores all the gun scriptable objects in the game, every time a new one is created, it must be added to this list via the Unity editor")]
    [SerializeField] private string savedFileName = "data";

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler fileDataHandler;

    public List<FurnitureSO> AllFurnitureSO { get => allFurnitureSOs; }

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
        return new(FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>()); 
    }

	public void NewGame()
    {
        gameData = new GameData();
        // Add the guns to the save
        gameData.gunSaveData.Add(new SaveDataGun("crossbow"));
        gameData.gunSaveData.Add(new SaveDataGun("rifle"));
        gameData.gunSaveData.Add(new SaveDataGun("shotgun"));
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
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public Placeable GetPlaceablePrefabById(string id)
    {
        return allFurnitureSOs.Find(x => x.id == id).placeablePrefab;
    }

    public GunSO GetGunById(string id) => allGunSOs.Find(x => x.id == id);
}
