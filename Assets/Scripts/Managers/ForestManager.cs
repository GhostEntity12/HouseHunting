using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ForestManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private float huntingDurationSeconds;
    [SerializeField] private TextMeshProUGUI huntingTimerText;

    private static ForestManager instance;
    private int currentHealth;
    private Inventory huntingInventory;
    private float huntingTimerSeconds;

    public static ForestManager Instance => instance;
    public Inventory HuntingInventory => huntingInventory;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        currentHealth = maxHealth;
        huntingTimerSeconds = huntingDurationSeconds;
    }

    private void Start()
    {
        huntingTimerText.text = FormatTime(huntingDurationSeconds);
    }

    private void Update() 
    {
        if (huntingTimerSeconds > 0)
        {
            huntingTimerSeconds -= Time.deltaTime;
            huntingTimerText.text = FormatTime(huntingTimerSeconds);
        }
        else
        {
            huntingTimerText.text = "00:00";
            RespawnInHouse();
        }
    }

    private void Die()
    {
        // clear the current hunting session's inventory
        huntingInventory.ClearInventory();

        // detach the camera from the player
        Camera camera = player.GetComponentInChildren<Camera>();
        camera.transform.parent = null;

        // destroy all children of the camera
        foreach (Transform child in camera.transform)
            Destroy(child.gameObject);
        
        // destroy the player object
        Destroy(player.gameObject);

        GameOver();
    }

    // converts seconds to a string in the format "mm:ss"
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    public void DealDamageToPlayer(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0) Die();
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        GameManager.Instance.ShowCursor();
    }

    public void RespawnInHouse()
    {
        gameOverUI.SetActive(false);
        GameManager.Instance.HideCursor();
        SceneManager.LoadScene("House");
    }

    public void LoadData(GameData data)
    {
        huntingInventory = Inventory.Deserialize(data.serializedHuntingInventory);
    }

    public void SaveData(GameData data)
    {
        data.serializedHuntingInventory = huntingInventory.Serialize();
    }
}
