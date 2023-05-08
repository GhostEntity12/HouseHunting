public struct InventoryItem
{
    public string id;
    public float scaleFactor;
    public int materialIndex;
    public int price;

    public InventoryItem(string id, float scaleFactor, int materialIndex, int price)
    {
        this.id = id;
        this.scaleFactor = scaleFactor;
        this.materialIndex = materialIndex;
        this.price = price;
    }

    public float value
    {
        get { return price * scaleFactor; }
    }
}
