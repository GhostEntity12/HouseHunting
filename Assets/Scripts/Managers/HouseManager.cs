using System.Collections.Generic;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
	public enum HouseMode { Explore, Decorate }
	public HouseMode Mode { get; private set; }

	public delegate void OnModeChange(HouseMode mode);
	public static event OnModeChange ModeChanged;

	[SerializeField] private CanvasGroup decorateUI;
	[SerializeField] private MeshFilter playerModel;
	[SerializeField] private GameObject playerGameObject;

	[field: SerializeField] public Camera ExploreCamera { get; private set; }
	[field: SerializeField] public Camera DecorateCamera { get; private set; }


	private List<HouseItem> houseItems;
	private float houseValue = 0;

	private void Start()
	{
		SpawnSerializedPlaceables();
		houseValue = CalculateHouseRating(houseItems); // assign total value here

        Debug.Log("HouseRating: "+houseValue);
		SetHouseMode(HouseMode.Explore);

		AudioManager.Instance.Play("Building");
	}

    private void Update()
    {
		// cast ray from center of the screen to 5 units away, project the selected furniture to the point
    }

    // function to calculate house rating, on certain threseholds (to be determined later), unlockTier is called to unlock that tier.
    private float CalculateHouseRating(List<HouseItem> houseItems)
	{
		float tValue = 0;
		foreach (HouseItem item in houseItems)
		{
			tValue += item.inventoryItem.Value;
		}
		// can be changed in future
		if (tValue > 9000)
			UnlockTier("D"); // dummy function for now
		return tValue;
	}

	public void UnlockTier(string tier)
	{
		// do nothing
	}

	public void LoadData(GameData data)
	{
		houseItems = data.houseItems;
	}
	public void SaveData(GameData data)
	{
		data.houseItems = houseItems;
	}

	void SpawnSerializedPlaceables()
	{
		foreach (HouseItem item in houseItems)
		{
			Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
			spawnedPlaceable.SetTransforms(item.position, item.rotationAngle);
			spawnedPlaceable.InventoryItem = item.inventoryItem;
		}
	}

	public void SavePlaceables()
	{
		Placeable[] allPlaceables = FindObjectsOfType<Placeable>();
		houseItems.Clear();
		foreach (Placeable placeable in allPlaceables)
		{
			MeshRenderer meshRenderer = placeable.GetComponentInChildren<MeshRenderer>();
			houseItems.Add(new HouseItem(placeable.InventoryItem, placeable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));
		}
	}

	/// <summary>
	/// Changes the mode the house is in from decorate to explore and vice versa
	/// </summary>
	/// <param name="mode"></param>
	public void SetHouseMode(HouseMode mode)
	{
		// dont allow to switch mode if the shop is opened
		if (ShopUIManager.Instance.IsShopOpen) return;
		// Need to:
		// - Swap camera
		// - Set cursor visibility
		// - Enable UI
		// - Hide/Show player model
		Mode = mode;
		switch (mode)
		{
			case HouseMode.Explore:
				ExploreCamera.enabled = true;
				DecorateCamera.enabled = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				decorateUI.alpha = 0;
				decorateUI.interactable = decorateUI.blocksRaycasts = false;
				playerModel.gameObject.SetActive(true);
				break;
			case HouseMode.Decorate:
				DecorateCamera.enabled = true;
				ExploreCamera.enabled = false;
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				decorateUI.alpha = 1;
				decorateUI.interactable = decorateUI.blocksRaycasts = true;
				playerModel.gameObject.SetActive(false);
				DecorateButtonGroupUIManager.Instance.ButtonGroupVisibility(false); // this is set to false because we only want to see the decorate button group when a furniture is selected
				break;
			default:
				break;
		}

		ModeChanged?.Invoke(mode);
	}

	public void SelectFurnitureToPlace()
	{
		if (ShopUIManager.Instance.SelectedFurniture?.so == null) return;
		(FurnitureSO so, FurnitureItem item) selectedFurniture = ShopUIManager.Instance.SelectedFurniture.Value;
		// cast ray from camera to 3 units away
		Placeable spawnedPlaceable = Instantiate(selectedFurniture.so.placeablePrefab);
		
		spawnedPlaceable.transform.position = playerGameObject.transform.forward * 3;
		// clamp the position so that the y index is always on ground level
		spawnedPlaceable.transform.position = new Vector3(spawnedPlaceable.transform.position.x, 0, spawnedPlaceable.transform.position.z);
		// rotate the furniture so that it faces the player
		spawnedPlaceable.transform.LookAt(ExploreCamera.transform.position);
		spawnedPlaceable.transform.rotation = Quaternion.Euler(spawnedPlaceable.transform.rotation.eulerAngles.x, 0, spawnedPlaceable.transform.rotation.eulerAngles.z);
		spawnedPlaceable.InventoryItem = selectedFurniture.item;

		ShopUIManager.Instance.ToggleShop();
	}
}
