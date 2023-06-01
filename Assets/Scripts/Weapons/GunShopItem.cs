public class GunShopItem : ShopItem
{
    public int bulletsInMag;

    // for json serialization
    public GunShopItem() : base()
    {
        bulletsInMag = 0;
    }

    public GunShopItem(string id, int quantity, int bulletsInMag) : base(id, quantity)
    {
        this.bulletsInMag = bulletsInMag;
    }

    public GunShopItem(string id) : base(id)
    {
        bulletsInMag = 0;
    }
}
