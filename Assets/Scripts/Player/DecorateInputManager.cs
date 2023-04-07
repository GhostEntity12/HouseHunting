using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DecorateInputManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private static DecorateInputManager instance;
    private PlayerInput playerInput;
    private bool mouseDown = false;
    private Placeable selectedPlaceable = null;

    public static DecorateInputManager Instance => instance;
    public Placeable SelectedPlaceable => selectedPlaceable;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        playerInput = new PlayerInput();

        playerInput.Decorate.ExitToHouse.performed += ctx => SceneManager.LoadScene("House");
        playerInput.Decorate.MouseDown.started += ctx => MousePressed();
        playerInput.Decorate.MouseDown.canceled += ctx => MouseUp();
    }

    void Start()
    {
        GameManager.Instance.ShowCursor();
    }

    private void OnEnable()
    {
        playerInput.Decorate.Enable();
    }

    private void OnDisable()
    {
        playerInput.Decorate.Disable();
    }

    private void OnDestroy()
    {
        GameManager.Instance.HideCursor();
    }

    private void MousePressed()
    {
        mouseDown = true;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Placeable placeable = hit.transform.GetComponent<Placeable>();
            SelectPlacable(placeable);
        }
    }

    private void MouseUp()
    {
        mouseDown = false;
    }

    private void Update()
    {
        if (mouseDown && selectedPlaceable != null)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // If the object we hit is a placeable, ignore collisions with it, we want the ray to go through it and hit the ground
                if (hit.transform.GetComponent<Placeable>() != null)
                {
                    Physics.IgnoreCollision(selectedPlaceable.GetComponent<Collider>(), hit.transform.GetComponent<Collider>());
                }
                else
                {
                    selectedPlaceable.transform.position = new Vector3(hit.point.x, selectedPlaceable.transform.position.y, hit.point.z);
                    selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = selectedPlaceable.IsValidPosition ? Color.green : Color.red;
                }
            }
        }
    }

    private void SelectPlacable(Placeable toBeSelected)
    {
        if (toBeSelected != null)
        {
            if (selectedPlaceable != null)
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            selectedPlaceable = toBeSelected;
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
        else //If raycast didn't hit a placeable, we want to deselect the current one
        {
            //Check if there is already a placeable selected
            if (selectedPlaceable == null) return;

            //If position of placeable is not valid, put it back in the inventory
            if (!selectedPlaceable.IsValidPosition)
            {
                GameManager.Instance.Inventory.AddItem(selectedPlaceable.PlaceableSO);
                InventoryUIManager.Instance.RepaintInventory();
                Destroy(selectedPlaceable.gameObject);
            }
            else
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            selectedPlaceable = null;
        }
    }
}
