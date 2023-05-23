using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyMenuUIManager : Singleton<BuyMenuUIManager>
{
    [SerializeField] private BuyMenuItemUI buyMenuItemPrefab;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private void OnEnable()
    {
        if (DataPersistenceManager.Instance != null)
        {
            foreach (ShopItemSO item in DataPersistenceManager.Instance.AllShopItems)
            {
                BuyMenuItemUI buyMenuItem = Instantiate(buyMenuItemPrefab, verticalLayoutGroup.transform);
                buyMenuItem.SetItem(item);
            }
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
    }

    public void ToggleBuyMenu(bool open)
    {
        if (open)
        {
            gameObject.SetActive(true);
            ShopUIManager.Instance.SelectItem(null);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
