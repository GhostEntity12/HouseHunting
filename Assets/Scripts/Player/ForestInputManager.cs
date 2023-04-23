using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestInputManager : MonoBehaviour
{
    private static ForestInputManager instance;
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;
    //private Gun gun;

    public static ForestInputManager Instance => instance;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerInput = new PlayerInput();

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        //gun = GetComponentInChildren<Gun>();

        camera = GetComponentInChildren<Camera>();

    }

    void FixedUpdate()
    {
        movement.Move(playerInput.Forest.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate() 
    {
        look.Look(playerInput.Forest.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerInput.Forest.Enable();
    }

    private void OnDisable()
    {
        playerInput.Forest.Disable();
    }

    
}
