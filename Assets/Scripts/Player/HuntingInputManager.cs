using UnityEngine;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
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
        playerInput.Hunting.Pause.performed += ctx => GameManager.Instance.SetGamePause(!GameManager.Instance.IsPaused);
    }

	private void FixedUpdate()
	{
		movement.Move(playerInput.Hunting.Movement.ReadValue<Vector2>());
		movement.Crouch(playerInput.Hunting.Crouch.ReadValue<float>());
		playerInput.Hunting.Jump.performed += ctx => movement.Jump();
	}

	private void LateUpdate()
	{
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
}
