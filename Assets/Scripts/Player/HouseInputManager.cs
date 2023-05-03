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

        playerInput = new PlayerInput();

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        camera = GetComponentInChildren<Camera>();

        playerInput.House.Interact.performed += ctx => Interact();
        playerInput.House.Decorate.performed += ctx => SceneManager.LoadScene("Decorate");
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
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 3f))
        {
            //if we are interacting with a door, load the forest scene
            if (hit.transform.parent.transform.CompareTag("Door"))
                SceneManager.LoadScene("ForestTestingKai");
        }
    }
}
