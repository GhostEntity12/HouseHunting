using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    [Header("Layouts Items")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image backgroundImageComponent;
    [SerializeField] private GridLayoutGroup furnitureItemContainer;
    [SerializeField] private RectTransform rightPanel;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI furnitureNameText;
    [SerializeField] private ModelPreview modelPreview;

    [Header("Tab Buttons")]
    [SerializeField] private Button livingTabButton;
    [SerializeField] private Button diningTabButton;
    [SerializeField] private Button bedroomTabButton;
    [SerializeField] private Button bathroomTabButton;
    [SerializeField] private Button miscTabButton;

    [Header("Inventory Pages")]
    [SerializeField] private Sprite inventoryPage1;
    [SerializeField] private Sprite inventoryPage2;
    [SerializeField] private Sprite inventoryPage3;
    [SerializeField] private Sprite inventoryPage4;
    [SerializeField] private Sprite inventoryPage5;

    [Header("Assets")]
    [SerializeField] private Sprite furnitureItemButtonBackground;

    private PlayerInput playerInput;

    private FurnitureType selectedTab;
    private (FurnitureSO so, SaveDataFurniture inventoryItem)? selectedFurniture;
    private FurnitureInventory furnitureInventory;

    public (FurnitureSO so, SaveDataFurniture inventoryItem)? SelectedFurniture 
    {
        get => selectedFurniture;
        set
        {
            if (value != null)
            {
                rightPanel.gameObject.SetActive(true);
                selectedFurniture = value;
                furnitureNameText.text = selectedFurniture.Value.so.name;
                modelPreview.MeshFilter.mesh = value.Value.so.placeablePrefab.MeshFilter.sharedMesh;
            }
            else
            {
                rightPanel.gameObject.SetActive(false);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        canvas.enabled = false;

        SelectedFurniture = null; // this is here to force the prop setter to set null

        // input
        playerInput = GeneralInputManager.Instance.PlayerInput;
        playerInput.Inventory.CloseInventory.performed += ctx => ToggleInventory();

        // button set up
        livingTabButton.onClick.AddListener(() => SetTab(FurnitureType.Living));
        diningTabButton.onClick.AddListener(() => SetTab(FurnitureType.Dining));
        bedroomTabButton.onClick.AddListener(() => SetTab(FurnitureType.Bedroom));
        bathroomTabButton.onClick.AddListener(() => SetTab(FurnitureType.Bathroom));
        miscTabButton.onClick.AddListener(() => SetTab(FurnitureType.Misc));
    }

    private void Start()
    {
        furnitureInventory = HuntingManager.Instance != null ? HuntingManager.Instance.HuntingInventory : GameManager.Instance.PermanentInventory;
        SetTab(0);
    }

    private void RedrawInventoryItems()
    {
        foreach (Transform child in furnitureItemContainer.transform)
            Destroy(child.gameObject);

        foreach (SaveDataFurniture savedFurniture in furnitureInventory.Furniture)
        {
            FurnitureSO savedFurnitureSO = DataPersistenceManager.Instance.AllFurnitureSO.Find(f => f.id == savedFurniture.id);
            if (savedFurnitureSO != null && savedFurnitureSO.type == selectedTab)
            {
                GameObject furnitureItem = new GameObject(savedFurnitureSO.id + " item");
                furnitureItem.transform.SetParent(furnitureItemContainer.transform);

                Button button = furnitureItem.AddComponent<Button>();
                button.onClick.AddListener(() => SelectedFurniture = (savedFurnitureSO, savedFurniture));

                Image buttonBackground = furnitureItem.AddComponent<Image>();
                buttonBackground.sprite = furnitureItemButtonBackground;

                Image furnitureItemImage = new GameObject(savedFurnitureSO.id + " sprite").AddComponent<Image>();
                furnitureItemImage.transform.parent = furnitureItem.transform;
                furnitureItemImage.sprite = savedFurnitureSO.thumbnail;
                furnitureItemImage.preserveAspect = true;
                furnitureItemImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    public void SetTab(FurnitureType tabToSet)
    {
        backgroundImageComponent.sprite = tabToSet switch
        {
            FurnitureType.Living => inventoryPage1,
            FurnitureType.Dining => inventoryPage2,
            FurnitureType.Bedroom => inventoryPage3,
            FurnitureType.Bathroom => inventoryPage4,
            FurnitureType.Misc => inventoryPage5,
            _ => throw new System.ArgumentOutOfRangeException("Setting Invalid Tab")
        };
        selectedTab = tabToSet;
        SelectedFurniture = null;
        RedrawInventoryItems();
    }

    public void ToggleInventory()
    {
        bool open = !canvas.enabled;

        canvas.enabled = open;

        GeneralInputManager.Instance.enabled = !open;
        if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = !open;
        else if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = !open;

        if (open)
        {
            playerInput.Inventory.Enable();
            GameManager.Instance.ShowCursor();
            RedrawInventoryItems();
        }
        else
        {
            playerInput.Inventory.Disable();
            GameManager.Instance.HideCursor();
        }
    }

    // referenced in discard button in inventory UI
    public void DiscardSelectedFurniture()
    {
        furnitureInventory.RemoveItem(selectedFurniture.Value.inventoryItem);
        SelectedFurniture = null;
        RedrawInventoryItems();
    }
}
