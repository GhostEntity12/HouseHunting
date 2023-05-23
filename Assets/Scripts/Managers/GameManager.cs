using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Canvas pauseMenuCanvas;

    private static GameManager instance;
    private Inventory permanentInventory;

    public static GameManager Instance => instance;
    public Inventory PermanentInventory { get => permanentInventory; }
    public int Currency { get; set; }
    public bool IsPaused { get => pauseMenuCanvas.enabled; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        HideCursor();
        permanentInventory = new Inventory();
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

    public void LoadData(GameData data)
    {
        permanentInventory.Items = data.permanentInventory;
        permanentInventory.GunAmmo = data.gunAmmo;
        Currency = data.currency;
    }

    public void SaveData(GameData data)
    {
        data.permanentInventory = permanentInventory.Items;
        data.gunAmmo = permanentInventory.GunAmmo;
        data.currency = Currency;
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
