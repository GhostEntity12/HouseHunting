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

    // This was changed to give a number of bullets by default (100)
    public WeaponInventoryItem(string id, int bullets = 100)
    {
        this.id = id;
        bulletsInMag = 0;
        totalBulletsLeft = 100;
    }
}