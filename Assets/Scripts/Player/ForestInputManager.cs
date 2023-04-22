using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestInputManager : MonoBehaviour
{
    private static ForestInputManager instance;
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;
    private Gun gun;

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

        camera = GetComponentInChildren<Camera>();
        gun = GetComponentInChildren<Gun>();

        playerInput.Forest.Shoot.performed += ctx => gun.Shoot();
        playerInput.Forest.Interact.performed += ctx => Interact();
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

    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 3f))
        {
            Shootable shootable = hit.transform.GetComponentInParent<Shootable>();
            if (shootable != null)
            {
                if (shootable.IsDead)
                {
                    GameManager.Instance.Inventory.AddItem(shootable.PlaceableSO);
                    Destroy(shootable.gameObject);
                }
            }
            //if we are interacting with a door, load the house scene
            if (hit.transform.parent.transform.CompareTag("Door"))
                SceneManager.LoadScene("House");
        }
    }
}
