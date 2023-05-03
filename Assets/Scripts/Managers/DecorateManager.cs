using System.Collections.Generic;
using UnityEngine;

public class DecorateManager : MonoBehaviour, IDataPersistence
{
    private static DecorateManager instance;
    private List<HouseItem> houseItems;

    public static DecorateManager Instance => instance;

    public void LoadData(GameData data)
    {
        houseItems = data.houseItems;
    }

    public void SaveData(GameData data)
    {
        data.houseItems = houseItems;
    }

    private void Awake() 
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        houseItems = new List<HouseItem>();
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

    public void SavePlaceables()
    {
        Placeable[] allPlaceables = FindObjectsOfType<Placeable>();
        houseItems.Clear();
        foreach (Placeable placeable in allPlaceables)
        {
            MeshRenderer meshRenderer = placeable.GetComponentInChildren<MeshRenderer>();
            houseItems.Add(new HouseItem(placeable.InventoryItem, placeable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));
        }
    }
}
