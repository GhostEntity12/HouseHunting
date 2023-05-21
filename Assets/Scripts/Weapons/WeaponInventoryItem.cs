public struct WeaponInventoryItem
{
    public string id;
    public int bulletsInMag;
    public int totalBulletsLeft;

    public WeaponInventoryItem(string id, int bulletsInMag, int totalBulletsLeft)
    {
        this.id = id;
        this.bulletsInMag = bulletsInMag;
        this.totalBulletsLeft = totalBulletsLeft;
    }

    public WeaponInventoryItem(string id)
    {
        this.id = id;
        bulletsInMag = 0;
        totalBulletsLeft = 0;
    }
}