public class GunShopItem : ShopItem
{
    public int bulletsInMag;
    public int totalBulletsLeft;

    // for json serialization
    public GunShopItem() : base()
    {
        bulletsInMag = 0;
        totalBulletsLeft = 0;
    }

    public GunShopItem(string id, int quantity, int bulletsInMag, int totalBulletsLeft) : base(id, quantity)
    {
        this.bulletsInMag = bulletsInMag;
        this.totalBulletsLeft = totalBulletsLeft;
    }

    public GunShopItem(string id) : base(id)
    {
        bulletsInMag = 0;
        totalBulletsLeft = 0;
    }
}
