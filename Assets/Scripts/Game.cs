using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game instance;
    private Inventory inventory;

    public static Game Instance { get; private set; }

    public Inventory Inventory { get => inventory; }

    private Game()
    {

    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory = new Inventory();

        //https://stackoverflow.com/questions/32306704/how-to-pass-data-and-references-between-scenes-in-unity
        DontDestroyOnLoad(gameObject);
    }

    public void AddItemToInventory(Placeable furniture)
    {
        inventory.AddItem(furniture);
        Debug.Log(inventory);
    }
}
