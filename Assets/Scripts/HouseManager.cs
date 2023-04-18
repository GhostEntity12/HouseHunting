using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour, IDataPersistence
{
    private static HouseManager instance;
    private List<SerializableDecoration> serializedDecorations;

    public static HouseManager Instance { get; private set; }

    public void LoadData(GameData data)
    {
        serializedDecorations = data.serializedDecorations;
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

        serializedDecorations = new List<SerializableDecoration>();
    }

    private void Start() 
    {
        List<(PlaceableSO placeableSO, SerializableDecoration serializableDecoration)> placeableSOsMap = SerializableDecoration.Deserialize(serializedDecorations);
        foreach ((PlaceableSO placeableSO, SerializableDecoration serializableDecoration) in placeableSOsMap)
        {
            Placeable spawnedPlaceable = Instantiate(placeableSO.placeablePrefab);
            spawnedPlaceable.transform.position = serializableDecoration.position;
            spawnedPlaceable.RotateToAngle(serializableDecoration.rotationAngle);
        }
    }
}
