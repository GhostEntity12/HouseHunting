using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
	[Header("Layouts Items")]
	[SerializeField] private Canvas canvas;
	[SerializeField] private RectTransform book;
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

	private PlayerInput playerInput;

	private FurnitureType selectedTab;
	private (FurnitureSO so, SaveDataFurniture inventoryItem)? selectedFurniture;
	private FurnitureInventory furnitureInventory;
	bool isVisible = false;

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
				modelPreview.SetModel(DataPersistenceManager.Instance.GetPlaceablePrefabById(SelectedFurniture.Value.inventoryItem.id).gameObject, NavMesh.GetSettingsByID(value.Value.so.shootablePrefab.GetComponent<NavMeshAgent>().agentTypeID).agentRadius);
			}
			else
			{
				rightPanel.gameObject.SetActive(false);
			}
		}
	}

	private int? GetNavMeshAgentID(string name)
	{
		for (int i = 0; i < NavMesh.GetSettingsCount(); i++)
		{
			NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(index: i);
			if (name == NavMesh.GetSettingsNameFromID(agentTypeID: settings.agentTypeID))
			{
				return settings.agentTypeID;
			}
		}
		return null;
	}

	protected override void Awake()
	{
		base.Awake();

		playerInput = GeneralInputManager.Instance.PlayerInput;

		SelectedFurniture = null; // this is here to force the prop setter to set null

		// button set up
		livingTabButton.onClick.AddListener(() => SetTab(FurnitureType.Living));
		diningTabButton.onClick.AddListener(() => SetTab(FurnitureType.Dining));
		bedroomTabButton.onClick.AddListener(() => SetTab(FurnitureType.Bedroom));
		bathroomTabButton.onClick.AddListener(() => SetTab(FurnitureType.Bathroom));
		miscTabButton.onClick.AddListener(() => SetTab(FurnitureType.Misc));

		playerInput.Inventory.Close.Disable();
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
				buttonBackground.color = new(0, 0, 0, 0);

				Image furnitureItemImage = new GameObject(savedFurnitureSO.id + " sprite").AddComponent<Image>();
				furnitureItemImage.transform.SetParent(furnitureItem.transform);
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
		// do not open inventory when holding furniture
		if (HouseManager.Instance && HouseManager.Instance.HoldingPlaceable != null) return;

		isVisible = !isVisible;

		if (isVisible)
		{
			LeanTween.moveY(book, 0, 0.3f).setEaseOutBack();
			GameManager.Instance.ShowCursor();
			RedrawInventoryItems();

			playerInput.Inventory.Close.Enable();
			playerInput.Inventory.Open.Disable();
			playerInput.Pause.Disable();
			playerInput.General.Disable();
			if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = false;
			if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = false;
		}
		else
		{
			LeanTween.moveY(book, -1080, 0.3f).setEaseInBack();
			GameManager.Instance.HideCursor();

			playerInput.Inventory.Open.Enable();
			playerInput.Inventory.Close.Disable();
			playerInput.Pause.Enable();
			playerInput.General.Enable();
			if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = true;
			if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = true;
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
