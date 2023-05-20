using UnityEngine;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] WeaponWheelController weaponWheelController;

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

		playerInput.Hunting.Interact.performed += ctx => Interact();

		playerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		playerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();
        playerInput.Hunting.Pause.performed += ctx => GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);
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
