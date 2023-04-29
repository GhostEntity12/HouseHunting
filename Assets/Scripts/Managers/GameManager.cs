using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    private static GameManager instance;
    private Inventory inventory;

    [SerializeField] private GameObject gameOverUI;

    public static GameManager Instance { get; private set; }

    public Inventory Inventory { get => inventory; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        HideCursor();
        inventory = new Inventory();
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
        inventory = Inventory.Deserialize(data.serializedInventory);
    }

    public void SaveData(GameData data)
    {
        data.serializedInventory = inventory.Serialize();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        ShowCursor();
    }

    public void RespawnInHouse()
    {
        gameOverUI.SetActive(false);
        HideCursor();
        SceneManager.LoadScene("House");
    }
}