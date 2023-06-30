using System.Collections.Generic;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
	public enum HouseMode { Explore, Decorate }
	public HouseMode Mode { get; private set; }

	[SerializeField] private CanvasGroup decorateUI;
	[SerializeField] private MeshFilter playerModel;
	[SerializeField] private GameObject playerGameObject;
    [field: SerializeField] public Camera ExploreCamera { get; private set; }
	[field: SerializeField] public Camera DecorateCamera { get; private set; }


	private Placeable holdingPlaceable;
    private float holdingPlaceableRotation = 0;
	private List<HouseItem> houseItems;
	private float houseValue = 0;

	public List<HouseItem> HouseItems => houseItems;
    public Placeable HoldingPlaceable { get => holdingPlaceable; set => holdingPlaceable = value; }
	public float HoldingPlaceableRotation { get => holdingPlaceableRotation; set => holdingPlaceableRotation = value; }

    public delegate void OnModeChange(HouseMode mode);
	public static event OnModeChange ModeChanged;


	private void Start()
	{
		SpawnSerializedPlaceables();
		houseValue = CalculateHouseRating(houseItems); // assign total value here

        Debug.Log("HouseRating: " + houseValue);
		SetHouseMode(HouseMode.Explore);

		AudioManager.Instance.Play("Building");
	}

    private void Update()
    {
        HoldPlaceable();
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

	private void HoldPlaceable()
	{
		if (holdingPlaceable == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Create a RaycastHit variable to store information about the hit
        RaycastHit hit;

		// Perform the raycast and check if it hits something within the specified distance
		if (Physics.Raycast(ray, out hit, 3, ~LayerMask.GetMask("Placeable")))
			holdingPlaceable.transform.position = new Vector3(hit.point.x,hit.point.y, hit.point.z);
		else
			holdingPlaceable.transform.position = playerGameObject.transform.position + playerGameObject.transform.forward * 3;

        // clamp the position so that the y index is always on ground level
        holdingPlaceable.transform.position = new Vector3(holdingPlaceable.transform.position.x, 0, holdingPlaceable.transform.position.z);
		// rotate the furniture so that it faces the player
		holdingPlaceable.transform.LookAt(ExploreCamera.transform.position);
		holdingPlaceable.transform.rotation = Quaternion.Euler(0, holdingPlaceable.transform.rotation.eulerAngles.y + holdingPlaceableRotation, 0);

		holdingPlaceable.Mesh.material.color = holdingPlaceable.IsValidPosition ? Color.green : Color.red;
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

	public void UpdatePlaceablesInHouse()
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

		holdingPlaceable = spawnedPlaceable;
        HoldPlaceable();

        spawnedPlaceable.InventoryItem = selectedFurniture.item;

		ShopUIManager.Instance.ToggleShop();
	}

	public void RotateHoldingPlaceable(float angle)
	{
		if (holdingPlaceable == null) return;

		holdingPlaceableRotation += angle;
	}

	public void PlaceHoldingPlaceable()
	{
		if (holdingPlaceable == null || !holdingPlaceable.IsValidPosition) return;

        GameManager.Instance.PermanentInventory.RemoveItem(holdingPlaceable.InventoryItem);

        MeshRenderer meshRenderer = holdingPlaceable.GetComponentInChildren<MeshRenderer>();
        houseItems.Add(new HouseItem(holdingPlaceable.InventoryItem, holdingPlaceable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));

		holdingPlaceable.Mesh.material = holdingPlaceable.Material;
		MeshCollider meshCollider = holdingPlaceable.GetComponentInChildren<MeshCollider>();
		meshCollider.enabled = true;
		holdingPlaceable = null;
		holdingPlaceableRotation = 0;
    }
}
