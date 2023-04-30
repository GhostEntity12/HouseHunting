using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    private static GameManager instance;
    private Inventory permanentInventory;

    [SerializeField] private GameObject gameOverUI;

    public static GameManager Instance { get; private set; }

    public Inventory PermanentInventory { get => permanentInventory; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

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
        permanentInventory = Inventory.Deserialize(data.serializedPermanentInventory);
    }

    public void SaveData(GameData data)
    {
        data.serializedPermanentInventory = permanentInventory.Serialize();
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
