using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour, IDataPersistence
{
    private static HouseManager instance;
    private List<HouseItem> houseItems;

    public static HouseManager Instance { get; private set; }

    public void LoadData(GameData data)
    {
        houseItems = data.houseItems;
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
        foreach (HouseItem item in houseItems)
        {
            Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
            spawnedPlaceable.transform.position = item.position;
            spawnedPlaceable.RotateToAngle(item.rotationAngle);
            spawnedPlaceable.InventoryItem = item.inventoryItem;
        }
    }
}
