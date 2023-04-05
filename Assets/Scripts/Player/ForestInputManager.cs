using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestInputManager : MonoBehaviour
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

        playerInput.Forest.Shoot.performed += ctx => gun.Shoot();
        playerInput.Forest.Interact.performed += ctx => Interact();

        //forest and house will have access to the same inventory
        Debug.Log(GameManager.Instance.Inventory);
    }

    // Update is called once per frame
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
            Shootable shootable = hit.transform.GetComponent<Shootable>();
            if (shootable != null)
            {
                if (shootable.IsDead)
                {
                    GameManager.Instance.Inventory.AddItem(shootable.Placeable);
                    Destroy(shootable.gameObject);
                }
            }
            if (hit.transform.CompareTag("Door"))
            {
                SceneManager.LoadScene("House");
            }
        }
    }
}
