using UnityEngine;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] WeaponWheel weaponWheelController;

	private new Camera camera;
	private PlayerInput playerInput;
	private PlayerMovement movement;
	private PlayerLook look;

	protected override void Awake()
	{
		playerInput = new PlayerInput();

		movement = GetComponent<PlayerMovement>();
		look = GetComponent<PlayerLook>();

		camera = GetComponentInChildren<Camera>();

		// interact
		playerInput.Hunting.Interact.performed += ctx => Interact();

		// weapon wheel
		playerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		playerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();

		// pause
        playerInput.Hunting.Pause.performed += ctx => GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);
		playerInput.Hunting.OpenInventory.performed += ctx => ShopUIManager.Instance.ToggleShop();
		playerInput.Hunting.Jump.performed += ctx => movement.Jump();

		// shoot
		playerInput.Hunting.Shoot.performed += ctx => WeaponManager.Instance.CurrentGun.Shoot();

		// reload
		playerInput.Hunting.Reload.performed += ctx => WeaponManager.Instance.CurrentGun.Reload();

		// select weapon
		playerInput.Hunting.Quick1.performed += ctx => WeaponManager.Instance.SelectGun(0);
		playerInput.Hunting.Quick2.performed += ctx => WeaponManager.Instance.SelectGun(1);
        playerInput.Hunting.Quick3.performed += ctx => WeaponManager.Instance.SelectGun(2);
        playerInput.Hunting.Quick4.performed += ctx => WeaponManager.Instance.SelectGun(3);
        playerInput.Hunting.Quick5.performed += ctx => WeaponManager.Instance.SelectGun(4);
        playerInput.Hunting.Quick6.performed += ctx => WeaponManager.Instance.SelectGun(5);

		//debug
		playerInput.Hunting.DebugAmmo.performed += ctx => WeaponManager.Instance.GiveAmmo(100);
    }

	private void FixedUpdate()
	{
		if (ShopUIManager.Instance.IsShopOpen) return;

		movement.Move(playerInput.Hunting.Movement.ReadValue<Vector2>());
		movement.Crouch(playerInput.Hunting.Crouch.ReadValue<float>());
	}

	private void LateUpdate()
	{ 
		if (ShopUIManager.Instance.IsShopOpen) return;
		if (!weaponWheelController.gameObject.activeInHierarchy)
			look.Look(playerInput.Hunting.Look.ReadValue<Vector2>());
	}

	private void OnEnable()
	{
		playerInput.Hunting.Enable();
	}

	private void OnDisable()
	{
		playerInput.Hunting.Disable();
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
}
