using System.Collections.Generic;

public class DecorateManager : Singleton<DecorateManager>, IDataPersistence
{
    private List<HouseItem> houseItems;


    public void LoadData(GameData data)
    {
        houseItems = data.houseItems;
    }

    public void SaveData(GameData data)
    {
        data.houseItems = houseItems;
    }

    //public void SavePlaceables()
    //{
    //    Placeable[] allPlaceables = FindObjectsOfType<Placeable>();
    //    houseItems.Clear();
    //    foreach (Placeable placeable in allPlaceables)
    //    {
    //        MeshRenderer meshRenderer = placeable.GetComponentInChildren<MeshRenderer>();
    //        houseItems.Add(new HouseItem(placeable.InventoryItem, placeable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));
    //    }
    //}

    //protected override void Awake() 
    //{
    //    base.Awake();
    //    houseItems = new List<HouseItem>();
    //}

    //private void Start() 
    //{
    //    foreach (HouseItem item in houseItems)
    //    {
    //        Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
    //        spawnedPlaceable.SetTransforms(item.position, item.rotationAngle);
    //        spawnedPlaceable.InventoryItem = item.inventoryItem;
    //    }
    //}
}
