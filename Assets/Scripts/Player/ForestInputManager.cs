using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestInputManager : MonoBehaviour
{
    private static ForestInputManager instance;
    private new Camera camera;
    private PlayerInput playerInput;
    private PlayerMovement movement;
    private PlayerLook look;

    public static ForestInputManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerInput = new PlayerInput();

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        camera = GetComponentInChildren<Camera>();

        playerInput.Forest.Interact.performed += ctx => Interact();
    }

    private void FixedUpdate()
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
                    ForestManager.Instance.HuntingInventory.AddItem(shootable.PlaceableSO);
                    Debug.Log(ForestManager.Instance.HuntingInventory);
                    Destroy(shootable.gameObject);
                }
            }
            //if we are interacting with a door, load the house scene
            if (hit.transform.parent.transform.CompareTag("Door"))
            {
                // add hunting inventory to the player inventory
                GameManager.Instance.PermanentInventory.MergeInventory(ForestManager.Instance.HuntingInventory);
                SceneManager.LoadScene("House");
            }
        }
    }
}
