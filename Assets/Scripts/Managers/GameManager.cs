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
        permanentInventory.SetInventory(data.permanentInventory);
        Currency = data.currency;
    }

    public void SaveData(GameData data)
    {
        data.permanentInventory = permanentInventory.Items;
        data.currency = Currency;
    }

    public void PauseGame()
    {
        if (pauseMenuCanvas == null) return;

        if (pauseMenuCanvas.enabled)
        {
            ResumeGame();
        }
        else
        {
            Time.timeScale = 0;
            ShowCursor();
            pauseMenuCanvas.enabled = true;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuCanvas == null) return;

        Time.timeScale = 1;
        HideCursor();
        pauseMenuCanvas.enabled = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        ShowCursor();
        SceneManager.LoadScene("MainMenu");
    }
}
