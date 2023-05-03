using System.Collections.Generic;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
    private static HouseManager instance;
    private List<HouseItem> houseItems;

    public void LoadData(GameData data)
    {
        houseItems = data.houseItems;
    }

    public void SaveData(GameData data) { }

    private void Start() 
    {
        SpawnSerializedPlaceables();
    }

    void SpawnSerializedPlaceables()
    {
		foreach (HouseItem item in houseItems)
		{
			Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
			spawnedPlaceable.SetTransforms(item.position, item.rotationAngle);
			spawnedPlaceable.InventoryItem = item.inventoryItem;
		}
	}
}
