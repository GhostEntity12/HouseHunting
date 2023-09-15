
public class HouseInputManager : Singleton<HouseInputManager>
{
	private PlayerInput playerInput;

	public PlayerInput PlayerInput => playerInput;

	protected override void Awake()
	{
		// Singleton setup
		base.Awake();

		// Generate inputs and subscribe
		playerInput = GeneralInputManager.Instance.PlayerInput;

		//removed for alpha
		playerInput.House.OpenInventory.performed += ctx => InventoryUIManager.Instance.ToggleInventory();
		playerInput.House.PlaceHoldingFurniture.performed += ctx => HouseManager.Instance.PlaceHoldingPlaceable();
		playerInput.House.RemoveHoldingFurniture.performed += ctx => RemoveHoldingPlaceable();
	}

    private void OnEnable()
    {
		playerInput.House.Enable();
    }

	private void LateUpdate()
	{
		float scroll = playerInput.House.RotateHoldingFurniture.ReadValue<float>();
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
	/// Removes the selected placeable from the scene and returns it to the inventory
	/// </summary>
	public void RemoveHoldingPlaceable()
	{
		if (!HouseManager.Instance.HoldingPlaceable) return;

		GameManager.Instance.PermanentInventory.AddItem(HouseManager.Instance.HoldingPlaceable.InventoryItem);
		Destroy(HouseManager.Instance.HoldingPlaceable.gameObject);
	}
}
