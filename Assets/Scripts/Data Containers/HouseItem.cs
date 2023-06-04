using UnityEngine;

public struct HouseItem
{
    public Vector3 position;
    public float rotationAngle;

    public FurnitureItem inventoryItem;

    public HouseItem(FurnitureItem inventoryItem, Vector3 position, float rotationAngle)
    {
        this.inventoryItem = inventoryItem;
        this.position = position;
        this.rotationAngle = rotationAngle;
    }
}