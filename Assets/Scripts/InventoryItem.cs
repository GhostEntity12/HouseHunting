public struct FurnitureItem
{
    public string id;
    public float scaleFactor;
    public int materialIndex;
    public int price;

    public float Value => price * scaleFactor;

    public FurnitureItem(string id, float scaleFactor, int materialIndex, int price)
    {
        this.id = id;
        this.scaleFactor = scaleFactor;
        this.materialIndex = materialIndex;
        this.price = price;
    }

}
