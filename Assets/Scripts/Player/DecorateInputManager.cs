using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecorateInputManager : Singleton<DecorateInputManager>
{
	[SerializeField] private new Camera camera;
	[SerializeField] private ScrollRect inventoryScrollView;
	[SerializeField] private float cameraSpeed = 20f;
	[SerializeField] private MeshRenderer floorMeshRenderer;

	private PlayerInput playerInput;
	private Placeable selectedPlaceable = null;
	private bool isDraggingPlaceable = false;
	private bool isDraggingCamera = false;
	private bool isSelectingPlaceable = false;
	private (Vector3 position, float angle) placeableInitialState = (Vector3.zero, 0f);
	private float cameraYRotation = 0f;
	private Bounds cameraBounds;

	public Placeable SelectedPlaceable => selectedPlaceable;

	protected override void Awake()
	{
		base.Awake();

		playerInput = new PlayerInput();

		playerInput.Decorate.ExitToHouse.performed += ctx =>
		{
			DecorateManager.Instance.SavePlaceables();
			SceneManager.LoadScene("House");
		};

		playerInput.Decorate.MouseDown.started += ctx => MouseDownStarted();
		playerInput.Decorate.MouseDown.canceled += ctx => MouseDownCanceled();
		playerInput.Decorate.MouseDown.performed += ctx => MouseDownPerformed();

		//camera can only move within 200% of the floor size
		cameraBounds = floorMeshRenderer.bounds;
		// doubles the size of the bounds
		cameraBounds.Expand(cameraBounds.size);
	}

	private void Start()
	{
		GameManager.Instance.ShowCursor();
	}

	private void OnEnable()
	{
		playerInput.Decorate.Enable();
	}

	private void OnDisable()
	{
		playerInput.Decorate.Disable();
	}

	private void OnDestroy()
	{
		GameManager.Instance.HideCursor();
	}

	private void MouseDownStarted()
	{
		// Returns true if a raycast hits a placeable which is the selected placeable
		isDraggingPlaceable = 
			Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, int.MaxValue, ~LayerMask.GetMask("Floor")) &&
			hit.transform.GetComponent<Placeable>() == selectedPlaceable;
	}

	private void MouseDownCanceled()
	{
		isDraggingCamera = false;
		isDraggingPlaceable = false;
		isSelectingPlaceable = false;
	}

	// On Mouse Up
	private void MouseDownPerformed()
	{
		if (selectedPlaceable) return;

		if (Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit) &&
			hit.transform.TryGetComponent(out Placeable placeable))
		{
			SelectPlacable(placeable);
		}
	}

	private void Update()
	{
		if (isDraggingPlaceable)
		{
			RaycastHit hitFloor = CastRayFromMouseToFloor();
			if (hitFloor.transform != null && selectedPlaceable != null)
			{
				Vector3 position = new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.WorldToScreenPoint(selectedPlaceable.transform.position).z);
				Vector3 worldPosition = camera.ScreenToWorldPoint(position);
				selectedPlaceable.transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
			}
		}
		//check if selected placeable is valid position every frame
		if (selectedPlaceable)
		{
			if (selectedPlaceable.IsValidPosition)
			{
				DecorateButtonGroupUIManager.Instance.OkButtonVisibility(true);
				selectedPlaceable.Mesh.material.color = Color.green;
			}
			else
			{
				DecorateButtonGroupUIManager.Instance.OkButtonVisibility(false);
				selectedPlaceable.Mesh.material.color = Color.red;
			}
		}
		//check if mouse is over UI, if not, drag camera
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			isDraggingCamera = !EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId) && !isDraggingPlaceable && !isSelectingPlaceable;
		}

		if (isDraggingCamera && selectedPlaceable && !selectedPlaceable.RotationWheel.IsRotating)
		{
			RotateCamera(playerInput.Decorate.MouseMove.ReadValue<Vector2>());
		}
	}

	private void FixedUpdate()
	{
		MoveCamera(playerInput.Decorate.MoveCamera.ReadValue<Vector2>());
	}

	private void MoveCamera(Vector2 input)
	{
		Vector3 direction = camera.transform.forward * input.y + camera.transform.right * input.x;
		direction.y = 0;

		Vector3 idealPosition = cameraBounds.ClosestPoint(camera.transform.position + cameraSpeed * Time.deltaTime * direction);

		camera.transform.position = new(idealPosition.x, camera.transform.position.y, idealPosition.z);
	}

	private RaycastHit CastRayFromMouseToFloor()
	{
		Vector3 screenMousePosFar = new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.farClipPlane);
		Vector3 screenMousePosNear = new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.nearClipPlane);
		Vector3 worldMousePosFar = camera.ScreenToWorldPoint(screenMousePosFar);
		Vector3 worldMousePosNear = camera.ScreenToWorldPoint(screenMousePosNear);

		Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out RaycastHit hit, int.MaxValue, 1 << LayerMask.NameToLayer("Floor"));

		return hit;
	}

	private void RotateCamera(Vector2 mouseDelta)
	{
		cameraYRotation -= mouseDelta.x * Time.deltaTime * 30;

		camera.transform.localRotation = Quaternion.Euler(camera.transform.localRotation.eulerAngles.x, cameraYRotation, 0);
	}

	/// <summary>
	/// Select a given placeable
	/// </summary>
	/// <param name="placeableToSelect"></param>
	public void SelectPlacable(Placeable placeableToSelect)
	{
		isSelectingPlaceable = true;

		//deselct previous placeable
		DeselectPlaceable();

		//set new placeable color to green
		selectedPlaceable = placeableToSelect;
		selectedPlaceable.Mesh.material.color = Color.green;

		//enable button group
		DecorateButtonGroupUIManager.Instance.ButtonGroupVisibility(true);

		//save initial state
		placeableInitialState.position = selectedPlaceable.transform.position;
		placeableInitialState.angle = selectedPlaceable.transform.rotation.y;

		//enable rotation wheel
		selectedPlaceable.RotationWheel.SetVisibility(true);

		//hide inventory
		inventoryScrollView.gameObject.SetActive(false);
	}

	public void DeselectPlaceable(bool savePosition = true)
	{
		if (selectedPlaceable == null) return;

		//if do not save position, put it back to initial state
		if (!savePosition)
		{
			selectedPlaceable.transform.position = placeableInitialState.position;
			selectedPlaceable.RotateToAngle(placeableInitialState.angle);
			selectedPlaceable.Mesh.material.color = Color.white;
		}
		else
		{
			//If position of placeable is not valid, put it back in the inventory
			if (!selectedPlaceable.IsValidPosition)
			{
				RemoveSelectedPlaceable();
				return;
			}
			else
			{
				selectedPlaceable.Mesh.material.color = Color.white;
			}
		}

		//disable rotation wheel
		selectedPlaceable.RotationWheel.SetVisibility(false);
		selectedPlaceable = null;

		inventoryScrollView.gameObject.SetActive(true);
	}

	/// <summary>
	/// Removes the selected placeable from the scene and returns it to the inventory
	/// </summary>
	public void RemoveSelectedPlaceable()
	{
		if (!selectedPlaceable) return;

        GameManager.Instance.PermanentInventory.AddItem(selectedPlaceable.InventoryItem);
        InventoryUIManager.Instance.RepaintInventory();
        Destroy(selectedPlaceable.gameObject);
        selectedPlaceable = null;
        inventoryScrollView.gameObject.SetActive(true);
    }
}
