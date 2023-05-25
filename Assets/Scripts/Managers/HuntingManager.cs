using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HuntingManager : Singleton<HuntingManager>, IDataPersistence
{
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private float huntingDurationSeconds;
    [SerializeField] private TextMeshProUGUI huntingTimerText;

    private int currentHealth;
    private Inventory huntingInventory;
    private float huntingTimerSeconds;
    public Inventory HuntingInventory => huntingInventory;

    private AudioManager audioManager;

    protected override void Awake()
    {
        base.Awake();

        currentHealth = maxHealth;
        huntingTimerSeconds = huntingDurationSeconds;

        huntingInventory = new Inventory();
    }

    private void Start()
    {
        huntingTimerText.text = FormatTime(huntingDurationSeconds);

        audioManager = FindObjectOfType<AudioManager>();
		audioManager.Play("Ambience");
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
        GameManager.Instance.PermanentInventory.MergeInventory(huntingInventory);
        gameOverUI.SetActive(false);
        GameManager.Instance.HideCursor();
        SceneManager.LoadScene("House");
    }

    public void LoadData(GameData data)
    {
        huntingInventory.SetInventory(data.huntingInventory);
    }

    public void SaveData(GameData data)
    {
        data.huntingInventory = huntingInventory.Items;
    }
}
