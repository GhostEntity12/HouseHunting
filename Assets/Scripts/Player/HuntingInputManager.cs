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

		// shoot
		playerInput.Hunting.Shoot.performed += ctx => WeaponManager.Instance.CurrentGun.Shoot();

		// reload
		playerInput.Hunting.Reload.performed += ctx => WeaponManager.Instance.CurrentGun.Reload();
    }

	private void FixedUpdate()
	{
		movement.Move(playerInput.Hunting.Movement.ReadValue<Vector2>());
		movement.Crouch(playerInput.Hunting.Crouch.ReadValue<float>());
	}

	private void LateUpdate()
	{
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
