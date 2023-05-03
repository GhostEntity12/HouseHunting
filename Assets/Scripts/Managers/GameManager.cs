using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    private static GameManager instance;
    private Inventory permanentInventory;

    public static GameManager Instance => instance;
    public Inventory PermanentInventory { get => permanentInventory; }

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
    }

    public void SaveData(GameData data)
    {
        data.permanentInventory = permanentInventory.Items;
    }
}
