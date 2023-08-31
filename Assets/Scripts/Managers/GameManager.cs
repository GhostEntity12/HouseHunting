using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private Canvas pauseMenuCanvas;
    public GameObject player;

    public FurnitureInventory PermanentInventory { get; private set; }
    public List<SaveDataGun> OwnedGuns { get; private set; }

    public int Currency { get; set; }
    public bool IsPaused { get => pauseMenuCanvas.enabled; }

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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	public void SetGamePause(bool pause)
    {
        if (pauseMenuCanvas == null) return;

        if (pause)
        {
            Time.timeScale = 0;
            ShowCursor();
            pauseMenuCanvas.enabled = true;
        }
        else
        {
            Time.timeScale = 1;
            HideCursor();
            pauseMenuCanvas.enabled = false;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        ShowCursor();
        SceneManager.LoadScene(0);
    }
}
