public struct InventoryItem
{
    public string id;
    public float scaleFactor;
    public int materialIndex;
    public int price;

    public float Value => price * scaleFactor;

    public InventoryItem(string id, float scaleFactor, int materialIndex, int price)
    {
        this.id = id;
        this.scaleFactor = scaleFactor;
        this.materialIndex = materialIndex;
        this.price = price;
    }

}
