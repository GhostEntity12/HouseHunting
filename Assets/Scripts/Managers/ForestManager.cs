using UnityEngine;

public class ForestManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform player;
    private static ForestManager instance;
    private int currentHealth;
    private Inventory huntingInventory;

    public static ForestManager Instance => instance;
    public Inventory HuntingInventory => huntingInventory;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        currentHealth = maxHealth;
    }

    public void DealDamageToPlayer(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0) Die();
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

        GameManager.Instance.GameOver();
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
