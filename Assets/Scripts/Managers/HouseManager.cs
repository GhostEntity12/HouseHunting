using System.Collections.Generic;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
	public enum HouseMode { Explore, Decorate }

	[SerializeField] private CanvasGroup decorateUI;
	[SerializeField] private MeshFilter playerModel;
	[field: SerializeField] public Camera ExploreCamera { get; private set; }
	[field: SerializeField] public Camera DecorateCamera { get; private set; }

	private List<HouseItem> houseItems;
	private float houseValue = 0;

	public HouseMode Mode { get; private set; }

	public delegate void OnModeChange(HouseMode mode);
	public static event OnModeChange ModeChanged;

	private void Start()
	{
		SpawnSerializedPlaceables();
		houseValue = CalculateHouseRating(houseItems); // assign total value here

        Debug.Log("HouseRating: " + houseValue);
		SetHouseMode(HouseMode.Explore);
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

	private void SpawnSerializedPlaceables()
	{
		foreach (HouseItem item in houseItems)
		{
			Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
			spawnedPlaceable.SetTransforms(item.position, item.rotationAngle);
			spawnedPlaceable.InventoryItem = item.inventoryItem;
		}
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
}
