using System.Collections.Generic;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
	[SerializeField] private CanvasGroup decorateUI;
	[SerializeField] private GameObject playerGameObject;
    [field: SerializeField] public Camera ExploreCamera { get; private set; }


	private Placeable holdingPlaceable;
    private float holdingPlaceableRotation = 0;
	private List<HouseItem> houseItems;
	private float houseValue = 0;

	public List<HouseItem> HouseItems => houseItems;
    public Placeable HoldingPlaceable { get => holdingPlaceable; set => holdingPlaceable = value; }
	public float HoldingPlaceableRotation { get => holdingPlaceableRotation; set => holdingPlaceableRotation = value; }

	private void Start()
	{
		SpawnSerializedPlaceables();
		houseValue = CalculateHouseRating(houseItems); // assign total value here

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

	public void SelectFurnitureToPlace()
	{
		if (ShopUIManager.Instance.SelectedFurniture?.so == null) return;
		(FurnitureSO so, FurnitureItem item) selectedFurniture = ShopUIManager.Instance.SelectedFurniture.Value;
		Placeable spawnedPlaceable = Instantiate(selectedFurniture.so.placeablePrefab);

		holdingPlaceable = spawnedPlaceable;
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
		holdingPlaceable.ChildMeshCollider.enabled = true;
		holdingPlaceable = null;
		holdingPlaceableRotation = 0;
    }
}
