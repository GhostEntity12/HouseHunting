using UnityEngine;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] WeaponWheel weaponWheelController;

	private new Camera camera;
	public PlayerInput PlayerInput { get; private set; }
	private PlayerMovement movement;
	private PlayerLook look;

	protected override void Awake()
	{
        base.Awake();

        PlayerInput = new PlayerInput();

		movement = GetComponent<PlayerMovement>();
		look = GetComponent<PlayerLook>();

		camera = GetComponentInChildren<Camera>();

		// interact
		PlayerInput.Hunting.Interact.performed += ctx => Interact();

		// weapon wheel
		PlayerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		PlayerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();

		// pause
        PlayerInput.Hunting.Pause.performed += ctx => GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);

		//removed for alpha
		//playerInput.Hunting.OpenInventory.performed += ctx => ShopUIManager.Instance.ToggleShop();
		PlayerInput.Hunting.Jump.performed += ctx => movement.Jump();

		// reload
		PlayerInput.Hunting.Reload.performed += ctx => WeaponManager.Instance.CurrentGun.Reload();

		// select weapon
		PlayerInput.Hunting.Quick1.performed += ctx => WeaponManager.Instance.SelectItem(0);
		PlayerInput.Hunting.Quick2.performed += ctx => WeaponManager.Instance.SelectItem(1);
        PlayerInput.Hunting.Quick3.performed += ctx => WeaponManager.Instance.SelectItem(2);
        PlayerInput.Hunting.Quick4.performed += ctx => WeaponManager.Instance.SelectItem(3);
        PlayerInput.Hunting.Quick5.performed += ctx => WeaponManager.Instance.SelectItem(4);
        PlayerInput.Hunting.Quick6.performed += ctx => WeaponManager.Instance.SelectItem(5);

		// debug
		PlayerInput.Hunting.DebugAmmo.performed += ctx => WeaponManager.Instance.GiveAmmo(100);

		// ADS
		PlayerInput.Hunting.ADS.performed += ctx => WeaponManager.Instance.CurrentGun.ToggleADS();
    }

	private void OnEnable()
	{
		PlayerInput.Hunting.Enable();
	}

	private void FixedUpdate()
	{
		if (ShopUIManager.Instance.IsShopOpen) return;

        movement.Move(PlayerInput.Hunting.Movement.ReadValue<Vector2>());
		movement.Crouch(PlayerInput.Hunting.Crouch.ReadValue<float>());
		movement.Run(PlayerInput.Hunting.Run.ReadValue<float>());
	}

	private void LateUpdate()
	{ 
		if (ShopUIManager.Instance.IsShopOpen) return;
		if (!weaponWheelController.gameObject.activeInHierarchy)
			look.Look(PlayerInput.Hunting.Look.ReadValue<Vector2>());
	}

	private void OnDisable()
	{
		PlayerInput.Hunting.Disable();
	}

    private void Interact()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 3f) && hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
			interactable.Interact();
        }
    }

	private void OpenWeaponWheel()
	{
		weaponWheelController.OpenWeaponWheel();
    }

	private void CloseWeaponWheel()
	{
		weaponWheelController.CloseWeaponWheel();
	}

	public bool WeaponWheelIsOpen()
	{
		return weaponWheelController.GetOpen();
	}

	/// <summary>
	/// Enables firing of the gun. Not done in awake to allow for setup (campfires) without firing weapon.
	/// </summary>
	public void EnableShooting() => PlayerInput.Hunting.Shoot.performed += ctx => WeaponManager.Instance.CurrentGun.Shoot();
}
