using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HouseInputManager : Singleton<HouseInputManager>
{
	[Header("Explore Mode")]
	[SerializeField] private PlayerMovement movement;
	[SerializeField] private PlayerLook look;
	[SerializeField] private float playerReach = 3f;

	[Header("Decorate Mode")]
	[SerializeField] private ScrollRect inventoryScrollView;
	[SerializeField] private float decorateCameraSpeed = 20f;
	[SerializeField] private MeshRenderer floorMeshRenderer;

	private PlayerInput playerInput;
	private bool isDraggingPlaceable = false;
	private bool isDraggingCamera = false;
	private bool isSelectingPlaceable = false;
	private (Vector3 position, float angle) placeableInitialState = (Vector3.zero, 0f);
	private Bounds cameraBounds;


	public Placeable SelectedPlaceable { get; private set; }

	protected override void Awake()
	{
		// Singleton setup
		base.Awake();

		// Subscribe to mode change event
		HouseManager.ModeChanged += SetInput;

		// Generate inputs and subscribe
		playerInput = new PlayerInput();

		playerInput.House.Interact.performed += ctx => ExploreInteract();
		playerInput.House.Decorate.performed += ctx => HouseManager.Instance.SetHouseMode(HouseManager.HouseMode.Decorate);
		playerInput.House.OpenShop.performed += ctx => ShopUIManager.Instance.ToggleShop();
		playerInput.House.Pause.performed += ctx => PausePressed();
		playerInput.House.PlaceFurniture.performed += ctx => HouseManager.Instance.PlaceHoldingPlaceable();

		playerInput.Decorate.MouseDown.started += ctx => DecorateMouseDownStarted();
		playerInput.Decorate.MouseDown.canceled += ctx => DecorateMouseDownCanceled();
		playerInput.Decorate.ExitToHouse.performed += ctx =>
		{
			HouseManager.Instance.UpdatePlaceablesInHouse();
			HouseManager.Instance.SetHouseMode(HouseManager.HouseMode.Explore);
		};

		// Set decorate camera bounds - only alow movement within 200% of the floor size
		cameraBounds = floorMeshRenderer.bounds;
		// doubles the size of the bounds
		cameraBounds.Expand(cameraBounds.size);

		playerInput.House.Enable();
	}

	private void FixedUpdate()
	{
		// Different moves for different modes
		switch (HouseManager.Instance.Mode)
		{
			case HouseManager.HouseMode.Explore:
				if (!ShopUIManager.Instance.IsShopOpen)
					movement.Move(playerInput.House.Movement.ReadValue<Vector2>());
				break;
			case HouseManager.HouseMode.Decorate:
				MoveDecorateCamera(playerInput.Decorate.MoveCamera.ReadValue<Vector2>());
				break;
			default:
				break;
		}
	}

	private void LateUpdate()
	{
		// 1st person camera movement for exploration mode
		if (ShopUIManager.Instance.IsShopOpen) return;
		
		look.Look(playerInput.House.Look.ReadValue<Vector2>());

		float scroll = playerInput.House.RotateFurniture.ReadValue<float>();
		if (scroll > 0f)
			HouseManager.Instance.RotateHoldingPlaceable(30f);
		else if (scroll < 0f)
			HouseManager.Instance.RotateHoldingPlaceable(-30f);
	}

	private void OnDestroy()
	{
		// Unsubscribe the listener and deactivate inputs
		HouseManager.ModeChanged -= SetInput;
		playerInput.House.Disable();
		playerInput.Decorate.Disable();
	}

	/// <summary>
	/// Handles uses of the interation key when in explore mode
	/// </summary>
	private void ExploreInteract()
	{
		if (Physics.Raycast(HouseManager.Instance.ExploreCamera.transform.position, HouseManager.Instance.ExploreCamera.transform.forward, out RaycastHit hit, playerReach))
		{
			if (hit.transform.TryGetComponent(out IInteractable interactable))
			{
				interactable.Interact();
			}
		}
	}

	/// <summary>
	/// Changes the set of inputs to use
	/// </summary>
	/// <param name="mode"></param>
	private void SetInput(HouseManager.HouseMode mode)
	{
		switch (mode)
		{
			case HouseManager.HouseMode.Explore:
				playerInput.House.Enable();
				playerInput.Decorate.Disable();
				break;
			case HouseManager.HouseMode.Decorate:
				playerInput.House.Disable();
				playerInput.Decorate.Enable();
				break;
			default:
				break;
		}
	}

	// Triggers on mouse down
	private void DecorateMouseDownStarted()
	{
		if (SelectedPlaceable)
		{
			// Sets isDraggingPlaceable to whether a raycast hits a placeable which is the selected placeable
			isDraggingPlaceable =
				Physics.Raycast(HouseManager.Instance.DecorateCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, int.MaxValue, ~LayerMask.GetMask("Floor")) &&
				hit.transform.TryGetComponent(out Placeable placeable) &&
				placeable == SelectedPlaceable;
		}
		// No selected raycast and clicked on a gameobject with a Placeable
		else if (Physics.Raycast(HouseManager.Instance.DecorateCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit) &&
			hit.transform.TryGetComponent(out Placeable placeable))
		{
			SelectPlacable(placeable);
		}
	}

	// Triggers on mouse up
	private void DecorateMouseDownCanceled()
	{
		isDraggingCamera = false;
		isDraggingPlaceable = false;
		isSelectingPlaceable = false;
	}

	/// <summary>
	/// Moves the camera in decorate mode
	/// </summary>
	/// <param name="input">The horizontal/vertical axes</param>
	private void MoveDecorateCamera(Vector2 input)
	{
		Vector3 direction = HouseManager.Instance.DecorateCamera.transform.forward * input.y + HouseManager.Instance.DecorateCamera.transform.right * input.x;
		direction.y = 0;

		Vector3 idealPosition = cameraBounds.ClosestPoint(HouseManager.Instance.DecorateCamera.transform.position + decorateCameraSpeed * Time.deltaTime * direction.normalized);

		HouseManager.Instance.DecorateCamera.transform.position = new(idealPosition.x, HouseManager.Instance.DecorateCamera.transform.position.y, idealPosition.z);
	}

	/// <summary>
	/// Rotates the camera in decorate mode
	/// </summary>
	/// <param name="mouseDelta">The change in mouse position</param>
	private void RotateDecorateCamera(Vector2 mouseDelta)
	{
		Camera camera = HouseManager.Instance.DecorateCamera;

		// rotate on the horizontal axis
		camera.transform.RotateAround(new Vector3(0, camera.transform.position.y, 0), Vector3.up, mouseDelta.x * Time.deltaTime * 30);

		// prevent the camera from rotating too much vertically, i.e, prevent it from seeing under the house's floor or flip around
		float verticalAngleToRotate = -mouseDelta.y * Time.deltaTime * 30;

		// Calculate distance from origin to point
		float distance = Vector3.Distance(Vector3.zero, camera.transform.position);

		// Calculate angle between y-axis and point
		float angle = Mathf.Atan2(camera.transform.position.y, Mathf.Sqrt(camera.transform.position.x * camera.transform.position.x + camera.transform.position.z * camera.transform.position.z)) * Mathf.Rad2Deg;

		if (angle + verticalAngleToRotate > 80 || angle + verticalAngleToRotate < 10) return;

		// rotate on the vertical axis
		camera.transform.RotateAround(Vector3.zero, camera.transform.right, verticalAngleToRotate);
	}

	/// <summary>
	/// Utility function to cast a ray from the mouse to the floor and return the hit, this is to know the position of the mouse on the floor game object
	/// </summary>
	private RaycastHit CastRayFromMouseToFloor()
	{
		Physics.Raycast(HouseManager.Instance.DecorateCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, int.MaxValue, 1 << LayerMask.NameToLayer("Floor"));

		return hit;
	}

	private void PausePressed()
	{
		if (!ShopUIManager.Instance.IsShopOpen)
		{
			GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);
		}
	}

	/// <summary>
	/// Select a given placeable
	/// </summary>
	/// <param name="placeableToSelect"></param>
	public void SelectPlacable(Placeable placeableToSelect)
	{
		isSelectingPlaceable = true;

		// Deselect previous placeable
		DeselectPlaceable();

		// Set new placeable color to green
		// This makes an assumption that the placement is automatically valid...
		// May be resolved by the every frame check
		SelectedPlaceable = placeableToSelect;
		SelectedPlaceable.Mesh.material.color = SelectedPlaceable.IsValidPosition ? Color.green : Color.red;

		// Enable button group
		DecorateButtonGroupUIManager.Instance.ButtonGroupVisibility(true);

		// Cache initial state
		placeableInitialState.position = SelectedPlaceable.transform.position;
		placeableInitialState.angle = SelectedPlaceable.transform.rotation.y;

		// Enable the placeable's rotation wheel
		SelectedPlaceable.RotationWheel.SetVisibility(true);

		// Hide inventory
		inventoryScrollView.gameObject.SetActive(false);

		// Dont allow player to change mode while placing furniture
		playerInput.Decorate.ExitToHouse.Disable();
	}

	public void DeselectPlaceable(bool savePosition = true)
	{
		if (!SelectedPlaceable) return;

		// Hide button group
		DecorateButtonGroupUIManager.Instance.ButtonGroupVisibility(false);

		// Re-enable house mode change
		playerInput.Decorate.ExitToHouse.Enable();
		// Revert mesh color
		SelectedPlaceable.Mesh.material = SelectedPlaceable.Material;
		// Disable rotation wheel
		SelectedPlaceable.RotationWheel.SetVisibility(false);

		if (savePosition)
		{
			//If position of placeable is not valid, put it back in the inventory
			//If position of placeable is not valid, put it back in the inventory
			if (SelectedPlaceable.IsValidPosition)
				inventoryScrollView.gameObject.SetActive(true);
			else
				RemoveSelectedPlaceable();
		}
		else
		{
			// If do not save position, revert to initial state
			SelectedPlaceable.SetTransforms(placeableInitialState.position, placeableInitialState.angle);
			SelectedPlaceable.Mesh.material = SelectedPlaceable.Material;
		}
		// Deselect
		SelectedPlaceable = null;
	}

	/// <summary>
	/// Removes the selected placeable from the scene and returns it to the inventory
	/// </summary>
	public void RemoveSelectedPlaceable()
	{
		if (!SelectedPlaceable) return;

		GameManager.Instance.PermanentInventory.AddItem(SelectedPlaceable.InventoryItem);
		InventoryUIManager.Instance.RepaintInventory();
		Destroy(SelectedPlaceable.gameObject);
		DeselectPlaceable(false);
		inventoryScrollView.gameObject.SetActive(true);
	}
}
