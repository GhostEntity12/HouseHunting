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
            RaycastHit hit = CastRay();
            if (hit.transform != null)
            {
                Vector3 position = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.WorldToScreenPoint(selectedPlaceable.transform.position).z);
                Vector3 worldPosition = camera.ScreenToWorldPoint(position);
                selectedPlaceable.transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = selectedPlaceable.IsValidPosition ? Color.green : Color.red;
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
            selectedPlaceable.GetComponentInChildren<Canvas>().enabled = true;
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
            {
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                selectedPlaceable.GetComponentInChildren<Canvas>().enabled = false;
            }
            selectedPlaceable = null;
        }
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.nearClipPlane);
        Vector3 worldMousePosFar = camera.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = camera.ScreenToWorldPoint(screenMousePosNear);

        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Floor"));

        return hit;
    }
}
