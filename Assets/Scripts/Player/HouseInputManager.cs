using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseInputManager : MonoBehaviour
{
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        camera = GetComponentInChildren<Camera>();

        playerInput.House.Interact.performed += ctx => Interact();
        playerInput.House.Decorate.performed += ctx => SceneManager.LoadScene("Decorate");

        //forest and house will have access to the same inventory
        Debug.Log(Game.Instance.Inventory);
    }

    // Update is called once per frame
    void FixedUpdate()
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
            if (hit.transform.CompareTag("Door"))
            {
                SceneManager.LoadScene("Forest");
            }
        }
    }
}
