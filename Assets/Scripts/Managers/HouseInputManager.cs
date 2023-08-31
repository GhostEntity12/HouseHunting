using UnityEngine;

public class HouseInputManager : Singleton<HouseInputManager>
{
	[SerializeField] private PlayerMovement movement;
	[SerializeField] private PlayerLook look;
	[SerializeField] private float playerReach = 3f;

	private PlayerInput playerInput;

	protected override void Awake()
	{
		// Singleton setup
		base.Awake();

		// Generate inputs and subscribe
		playerInput = new PlayerInput();

		playerInput.House.Interact.performed += ctx => Interact();
		//removed for alpha
		playerInput.House.OpenShop.performed += ctx => ShopUIManager.Instance.ToggleShop();
		playerInput.House.Pause.performed += ctx => PausePressed();
		playerInput.House.PlaceFurniture.performed += ctx => HouseManager.Instance.PlaceHoldingPlaceable();
		playerInput.House.RemoveFurniture.performed += ctx => RemoveHoldingPlaceable();
	}

    private void OnEnable()
    {
		playerInput.House.Enable();
    }

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.transform.TryGetComponent(out IInteractable interactable) && interactable.Interactable)
        {
			InteractPopupManager.Instance.gameObject.SetActive(true);
			InteractPopupManager.Instance.SetAction(interactable.InteractActionText);
        } else
		{
			InteractPopupManager.Instance.gameObject.SetActive(false);
		}
    }

    private void FixedUpdate()
	{
        if (!ShopUIManager.Instance.IsShopOpen)
            movement.Move(playerInput.House.Movement.ReadValue<Vector2>());
			movement.Crouch(playerInput.Hunting.Crouch.ReadValue<float>());
			movement.Run(playerInput.Hunting.Run.ReadValue<float>());
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

	private void OnDisable()
	{
		playerInput.House.Disable();
	}

	/// <summary>
	/// Handles uses of the interation key when in explore mode
	/// </summary>
	private void Interact()
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
	public void RemoveHoldingPlaceable()
	{
		if (!HouseManager.Instance.HoldingPlaceable) return;

		GameManager.Instance.PermanentInventory.AddItem(HouseManager.Instance.HoldingPlaceable.InventoryItem);
		Destroy(HouseManager.Instance.HoldingPlaceable.gameObject);
	}
}
