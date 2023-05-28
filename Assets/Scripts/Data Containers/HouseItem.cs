using UnityEngine;

public struct HouseItem
{
    public Vector3 position;
    public float rotationAngle;
    public InventoryItem inventoryItem;

    public HouseItem(InventoryItem inventoryItem, Vector3 position, float rotationAngle)
    {
        this.inventoryItem = inventoryItem;
        this.position = position;
        this.rotationAngle = rotationAngle;
    }
}