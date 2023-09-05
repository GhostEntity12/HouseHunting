using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private Player player;

    public FurnitureInventory PermanentInventory { get; private set; }
    public List<SaveDataGun> OwnedGuns { get; private set; }
    public Player Player => player;

    public int Currency { get; set; }

    protected override void Awake()
    {
        base.Awake();

        HideCursor();
        PermanentInventory = new();
    }

	public void SaveData(GameData data)
	{
		data.storedFurniture = PermanentInventory.Furniture;
        data.gunSaveData = OwnedGuns;
		data.currency = Currency;
	}

	public void LoadData(GameData data)
    {
        PermanentInventory.Furniture = data.storedFurniture;
        OwnedGuns = data.gunSaveData;
        Currency = data.currency;
    }
    
	public void HideCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    public void ShowCursor()
	{
		//Cursor.lockState = CursorLockMode.None;
		//Cursor.visible = true;
	}

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        ShowCursor();
        SceneManager.LoadScene(0);
    }
}
