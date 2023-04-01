using UnityEngine;

public class InputManager : MonoBehaviour
{
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

        gun = GetComponentInChildren<Gun>();

        playerInput.Player.Shoot.performed += ctx => gun.Shoot();
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
}
