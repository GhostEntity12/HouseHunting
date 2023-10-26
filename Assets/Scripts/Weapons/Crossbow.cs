public class Crossbow : Gun
{
    public override void Shoot()
    {
        base.Shoot();
        Invoke(nameof(Reload), GunSO.timeBetweenShots);
    }
}
