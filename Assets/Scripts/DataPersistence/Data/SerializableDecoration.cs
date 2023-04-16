using System.Collections.Generic;
using UnityEngine;

public struct SerializableDecoration
{
    public string id;
    public Vector3 position;
    public float rotationAngle;

    public SerializableDecoration(string id, Vector3 position, float rotationAngle)
    {
        this.id = id;
        this.position = position;
        this.rotationAngle = rotationAngle;
    }

    public static List<(PlaceableSO placeableSO, SerializableDecoration serializableDecoration)> Deserialize(List<SerializableDecoration> serializedDecorations)
    {
        List<(PlaceableSO, SerializableDecoration)> placeableSOsMap = new List<(PlaceableSO, SerializableDecoration)>();
        foreach (SerializableDecoration serializedDecoration in serializedDecorations)
        {
            PlaceableSO placeableSO = DataPersistenceManager.Instance.PlaceableScriptableObjects.Find(x => x.id == serializedDecoration.id);
            placeableSOsMap.Add((placeableSO, serializedDecoration));
        }
        return placeableSOsMap;
    }
}