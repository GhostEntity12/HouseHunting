using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseManager : Singleton<HouseManager>, IDataPersistence
{
	[SerializeField] private RectTransform furnitureDecorateTooltips;
	[SerializeField] private RectTransform inventoryTooltip;
	[SerializeField] private CanvasGroup fade;

	[field: SerializeField] public Camera ExploreCamera { get; private set; }

	private bool decorateTooltipsActive = false;
	private Placeable holdingPlaceable;
	private List<SaveDataPlacedFurniture> houseItems;
	private Player player;
	private float holdingPlaceableRotation = 0;
	private Color holdingPlaceableOriginalColor = Color.white;
	private float houseValue = 0;

	public List<SaveDataPlacedFurniture> HouseItems => houseItems;
	public Placeable HoldingPlaceable
	{
		get => holdingPlaceable;
		set
		{
			holdingPlaceable = value;
			holdingPlaceableOriginalColor = holdingPlaceable.MeshRenderer.material.color;
		}
	}
	public float HoldingPlaceableRotation { get => holdingPlaceableRotation; set => holdingPlaceableRotation = value; }

	private void Start()
	{
		SpawnSerializedPlaceables();
		houseValue = CalculateHouseRating(houseItems); // assign total value here
		player = GameManager.Instance.Player;
		AudioManager.Instance.Play("Building");
		LeanTween.alphaCanvas(fade, 0, 0.3f);
	}

	private void Update()
	{
		HoldPlaceable();
	}

	// function to calculate house rating, on certain threseholds (to be determined later), unlockTier is called to unlock that tier.
	private float CalculateHouseRating(List<SaveDataPlacedFurniture> houseItems)
	{
		float tValue = houseItems.Count;

		// can be changed in future
		if (tValue > 9000)
			UnlockTier("D"); // dummy function for now

		return tValue;
	}

	private void SpawnSerializedPlaceables()
	{
		foreach (SaveDataPlacedFurniture item in houseItems)
		{
			Placeable spawnedPlaceable = Instantiate(DataPersistenceManager.Instance.GetPlaceablePrefabById(item.inventoryItem.id));
			spawnedPlaceable.SetTransforms(item.position, item.rotationAngle);
			spawnedPlaceable.InventoryItem = item.inventoryItem;
		}
	}

	private void SetTooltipVisibility(bool visible)
	{
		if (visible)
		{
			LeanTween.moveX(inventoryTooltip.gameObject, -200 * inventoryTooltip.transform.parent.localScale.x, 0.3f).setEaseInBack().setOnComplete(() =>
				LeanTween.moveX(furnitureDecorateTooltips.gameObject, 20 * inventoryTooltip.transform.parent.localScale.x, 0.3f).setEaseOutBack());
		}
		else
		{
			LeanTween.moveX(furnitureDecorateTooltips.gameObject, -200 * furnitureDecorateTooltips.transform.parent.localScale.x, 0.3f).setEaseInBack().setOnComplete(() =>
				LeanTween.moveX(inventoryTooltip.gameObject, 20 * furnitureDecorateTooltips.transform.parent.localScale.x, 0.3f).setEaseOutBack());
		}
	}

	/// <summary>
	/// This should run every frame to make the held furniture follow the player
	/// </summary>
	private void HoldPlaceable()
	{
		if (!holdingPlaceable && decorateTooltipsActive)
		{
			SetTooltipVisibility(false);
			decorateTooltipsActive = false;
			return;
		}
		else if (holdingPlaceable && !decorateTooltipsActive)
		{
			SetTooltipVisibility(true);
			decorateTooltipsActive = true;
		}
		else if (!holdingPlaceable) return;

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

		// Perform the raycast and check if it hits something within the specified distance
		int baseMask = LayerMask.GetMask("Floor") | LayerMask.GetMask("House Wall");

		// if hit floor or wall, set y the position to 0
		// else if holding placeable can place on surface and hit on surface, set y to the height of the surface
		// else set the position to be 3 units in front of player
		if (Physics.Raycast(ray, out RaycastHit hitFloorOrWall, 3, baseMask))
			holdingPlaceable.transform.position = new Vector3(hitFloorOrWall.point.x, player.transform.position.y, hitFloorOrWall.point.z);
		else if (Physics.Raycast(ray, out RaycastHit hit2, GameManager.Instance.Player.InteractRange, LayerMask.GetMask("PlaceableSurface")) && holdingPlaceable.CanPlaceOnSurface)
			holdingPlaceable.transform.position = new Vector3(hit2.point.x, hit2.point.y, hit2.point.z);
		else
			holdingPlaceable.transform.position = player.transform.position + player.transform.forward * 3;

        // clamp the position so that the y index is always on ground level
        holdingPlaceable.transform.position = new Vector3(holdingPlaceable.transform.position.x, holdingPlaceable.transform.position.y, holdingPlaceable.transform.position.z);
		//holdingPlaceable.transform.position = new Vector3(holdingPlaceable.transform.position.x, 0, holdingPlaceable.transform.position.z);
		// rotate the furniture so that it faces the player
		holdingPlaceable.transform.LookAt(ExploreCamera.transform.position);
		holdingPlaceable.transform.rotation = Quaternion.Euler(0, holdingPlaceable.transform.rotation.eulerAngles.y + holdingPlaceableRotation, 0);

		holdingPlaceable.MeshRenderer.material.color = holdingPlaceable.IsValidPosition ? Color.green : Color.red;
	}

	public void UnlockTier(string tier)
	{
		// do nothing
	}

	public void LoadData(GameData data)
	{
		houseItems = data.placedFurniture;
	}

	public void SaveData(GameData data)
	{
		data.placedFurniture = houseItems;
	}

	public void PlaceSelectedFurnitureFromInventory()
	{
		if (InventoryUIManager.Instance.SelectedFurniture?.so == null) return;
		InventoryUIManager.Instance.ToggleInventory();
		(FurnitureSO so, SaveDataFurniture item) selectedFurniture = InventoryUIManager.Instance.SelectedFurniture.Value;
		InventoryUIManager.Instance.SelectedFurniture = null;
		Placeable spawnedPlaceable = Instantiate(selectedFurniture.so.placeablePrefab);

		holdingPlaceable = spawnedPlaceable;
		MeshCollider meshCollider = holdingPlaceable.GetComponentInChildren<MeshCollider>();
		meshCollider.enabled = false;
		spawnedPlaceable.InventoryItem = selectedFurniture.item;


		GameManager.Instance.PermanentInventory.RemoveItem(holdingPlaceable.InventoryItem);
	}

	public void RotateHoldingPlaceable(float angle)
	{
		if (holdingPlaceable == null) return;

		holdingPlaceableRotation += angle;
	}

	public void PlaceHoldingPlaceable()
	{
		if (holdingPlaceable == null || !holdingPlaceable.IsValidPosition) return;

		MeshRenderer meshRenderer = holdingPlaceable.GetComponentInChildren<MeshRenderer>();
		houseItems.Add(new SaveDataPlacedFurniture(holdingPlaceable.InventoryItem, holdingPlaceable.transform.position, meshRenderer.transform.rotation.eulerAngles.y));

		holdingPlaceable.MeshRenderer.material.color = holdingPlaceableOriginalColor;
		holdingPlaceable.ChildMeshCollider.enabled = true;
		holdingPlaceable = null;
		holdingPlaceableRotation = 0;
	}

	public void LoadHuntingScene()
	{
		SceneManager.LoadSceneAsync("99_LoadingScene", LoadSceneMode.Additive);
		LeanTween.alphaCanvas(fade, 1, 0.5f).setOnComplete(() =>
			SceneManager.LoadSceneAsync(2));
	}
}
