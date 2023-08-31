public struct SaveDataFurniture
{
    public string id;
    public float scaleFactor;
    public int materialIndex;
    public int price;

    public float Value => price * scaleFactor;

    public SaveDataFurniture(string id, float scaleFactor, int materialIndex, int price)
    {
        this.id = id;
        this.scaleFactor = scaleFactor;
        this.materialIndex = materialIndex;
        this.price = price;
    }

}
