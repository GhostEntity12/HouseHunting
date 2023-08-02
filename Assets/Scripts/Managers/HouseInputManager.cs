using UnityEngine;

public class HouseInputManager : Singleton<HouseInputManager>
{
	[SerializeField] private PlayerMovement movement;
	[SerializeField] private PlayerLook look;
	[SerializeField] private float playerReach = 3f;

	private PlayerInput playerInput;

	public Placeable SelectedPlaceable { get; private set; }

	protected override void Awake()
	{
		// Singleton setup
		base.Awake();

		// Generate inputs and subscribe
		playerInput = new PlayerInput();

		playerInput.House.Interact.performed += ctx => ExploreInteract();
		//removed for alpha
		playerInput.House.OpenShop.performed += ctx => ShopUIManager.Instance.ToggleShop();
		playerInput.House.Pause.performed += ctx => PausePressed();
		playerInput.House.PlaceFurniture.performed += ctx => HouseManager.Instance.PlaceHoldingPlaceable();

		playerInput.House.Enable();
	}

	private void FixedUpdate()
	{
        if (!ShopUIManager.Instance.IsShopOpen)
            movement.Move(playerInput.House.Movement.ReadValue<Vector2>());
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
		playerInput.House.Disable();
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

	private void PausePressed()
	{
		if (!ShopUIManager.Instance.IsShopOpen)
		{
			GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);
		}
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
	}
}
