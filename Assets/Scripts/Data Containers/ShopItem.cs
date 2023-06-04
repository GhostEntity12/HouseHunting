public class ShopItem
{
    public string id;
    public int quantity;

    // for json serialization
    public ShopItem()
    {
        id = "";
        quantity = 0;
    }

    public ShopItem(string id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }

    // This was changed to give a number of bullets by default (100)
    public ShopItem(string id)
    {
        this.id = id;
        quantity = 0;
    }
}