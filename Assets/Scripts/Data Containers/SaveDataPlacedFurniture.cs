using UnityEngine;

public struct SaveDataPlacedFurniture
{
    public Vector3 position;
    public float rotationAngle;

    public SaveDataFurniture inventoryItem;

    public SaveDataPlacedFurniture(SaveDataFurniture inventoryItem, Vector3 position, float rotationAngle)
    {
        this.inventoryItem = inventoryItem;
        this.position = position;
        this.rotationAngle = rotationAngle;
    }
}