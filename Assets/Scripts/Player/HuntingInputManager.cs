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
	}

	private void FixedUpdate()
	{
		movement.Move(playerInput.Hunting.Movement.ReadValue<Vector2>());
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
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 3f))
		{
			if (hit.transform.TryGetComponent(out Shootable shootable) && shootable.IsDead)
			{
				HuntingManager.Instance.HuntingInventory.AddItem(shootable.GetInventoryItem());
				Debug.Log(HuntingManager.Instance.HuntingInventory);
				Destroy(shootable.gameObject);
			}
			//if we are interacting with a door, load the house scene
			else if (hit.transform.parent.transform.CompareTag("Door"))
				HuntingManager.Instance.RespawnInHouse();
		}
	}
}
