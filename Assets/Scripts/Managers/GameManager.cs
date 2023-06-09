using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private Canvas pauseMenuCanvas;
    public GameObject player;

    private static GameManager instance;
    private Inventory permanentInventory;

    public Inventory PermanentInventory { get => permanentInventory; }
    public int Currency { get; set; }
    public bool IsPaused { get => pauseMenuCanvas.enabled; }

    protected override void Awake()
    {
        base.Awake();

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
        permanentInventory.BoughtItems = data.boughtItems;
        Currency = data.currency;
    }

    public void SaveData(GameData data)
    {
        data.permanentInventory = permanentInventory.Items;
        List<ShopItem> boughtItems = permanentInventory.BoughtItems;
        // if a gun is a ShopItem instead of a GunShopItem, convert it to GunShopItem
        for (int i = 0; i < boughtItems.Count; i++)
        {
            if (boughtItems[i] is GunShopItem) continue;
            if (WeaponManager.Instance.AllGuns.Any(x => x.GunSO.id == boughtItems[i].id))
            {
                GunShopItem gunShopItem = new GunShopItem(boughtItems[i].id, boughtItems[i].quantity, 0);
                boughtItems[i] = gunShopItem;
            }
        }
        data.boughtItems = permanentInventory.BoughtItems;
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
