using UnityEngine;

[CreateAssetMenu(fileName = "GunSO")]
public class GunSO : ScriptableObject 
{
    public string id;
    public new string name;
    public Bullet bulletPrefab;
    public Sprite icon;
    public float shootForce;
    public float timeBetweenShots;
    public float spread;
    public float reloadTime;
    public int magSize;
    public int bulletsPerTap;
}