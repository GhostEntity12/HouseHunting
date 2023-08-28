[System.Serializable]
public class AmmoPouch
{
	public int AmmoStored { get; private set; }
	public int AmmoInGun { get ; private set; }

	public int LoadGun(int capacity)
	{
		int ammoToLoad = capacity - AmmoInGun;
		AmmoStored -= ammoToLoad;
		AmmoInGun += ammoToLoad;
		return AmmoInGun;
	}

	public void AddAmmo(int count) => AmmoStored += count;
	public void RemoveAmmo(int count) => AmmoInGun -= count;

}
