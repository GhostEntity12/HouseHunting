using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseInputManager : Singleton<HouseInputManager>
{
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;


    protected override void Awake()
    {
        // Singleton setup
        base.Awake();

        // Generate inputs and subscribe
        playerInput = new PlayerInput();
        playerInput.House.Interact.performed += ctx => Interact();
        playerInput.House.Decorate.performed += ctx => SceneManager.LoadScene("Decorate");

        // Get movement and look
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        // Get camera
        camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        movement.Move(playerInput.House.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate() 
    {
        look.Look(playerInput.House.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerInput.House.Enable();
    }

    private void OnDisable()
    {
        playerInput.House.Disable();
    }

    private void Interact()
    {
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 3f) &&
            hit.transform.parent.CompareTag("Door"))
        {
			SceneManager.LoadScene("ForestTesting");
        }
	}
}
