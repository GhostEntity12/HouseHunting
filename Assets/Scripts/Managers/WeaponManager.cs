using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private List<Gun> guns;
    private Gun currentGun;
    private int currentGunIndex = 0;

    public List<Gun> Guns => guns;
    public int CurrentGunIndex => currentGunIndex;

    protected override void Awake()
    {
        base.Awake();

        currentGun = Instantiate(guns[0], transform);
    }

    public void SelectGun(int index)
    {
        Gun selectedGun = guns[index];
        if (selectedGun != null && selectedGun != currentGun)
        {
            Destroy(currentGun.gameObject);
            currentGun = Instantiate(selectedGun, transform);
            currentGunIndex = index;
        }
    }
}
