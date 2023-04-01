using UnityEngine;

public class InputManager : MonoBehaviour
{
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;
    private Gun gun;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        camera = GetComponentInChildren<Camera>();
        gun = GetComponentInChildren<Gun>();

        playerInput.Player.Shoot.performed += ctx => gun.Shoot();
        playerInput.Player.Interact.performed += ctx => Interact();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement.Move(playerInput.Player.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate() 
    {
        look.Look(playerInput.Player.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }

    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 3f))
        {
            Shootable shootable = hit.transform.GetComponent<Shootable>();
            if (shootable != null)
            {
                if (shootable.IsDead)
                {
                    Game.Instance.AddItemToInventory(shootable.Placeable);
                    Destroy(shootable.gameObject);
                }
            }
        }
    }
}
