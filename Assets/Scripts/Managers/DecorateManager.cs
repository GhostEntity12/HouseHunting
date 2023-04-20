using System.Collections.Generic;
using UnityEngine;

public class DecorateManager : MonoBehaviour, IDataPersistence
{
    private static DecorateManager instance;

    public static DecorateManager Instance { get; private set; }

    private List<SerializableDecoration> serializedDecorations;

    public void LoadData(GameData data)
    {
        serializedDecorations = data.serializedDecorations;
    }

    public void SaveData(GameData data)
    {
        data.serializedDecorations = serializedDecorations;
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

    public void SavePlaceables()
    {
        Placeable[] allPlaceables = FindObjectsOfType<Placeable>();
        serializedDecorations.Clear();
        foreach (Placeable placeable in allPlaceables)
        {
            MeshRenderer meshRenderer = placeable.GetComponentInChildren<MeshRenderer>();
            serializedDecorations.Add(new SerializableDecoration(placeable.PlaceableSO.id, placeable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));
        }
    }
}
