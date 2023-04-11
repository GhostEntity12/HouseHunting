using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecorateInputManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private ScrollRect inventoryScrollView;

    private static DecorateInputManager instance;
    private PlayerInput playerInput;
    private Placeable selectedPlaceable = null;
    private bool isDraggingPlaceable = false;
    private bool mouseDown = false;
    private (Vector3 position, float angle) placeableInitialState = (Vector3.zero, 0f);

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

        playerInput.Decorate.MouseDown.started += ctx => MouseDownStarted();
        playerInput.Decorate.MouseDown.canceled += ctx => MouseDownCanceled();

        playerInput.Decorate.MouseDown.performed += ctx => MouseDownPerformed();
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

    private void MouseDownStarted()
    {
        mouseDown = true;
        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
        if (hit.transform != null && hit.transform.GetComponent<Placeable>() == selectedPlaceable)
        {
            isDraggingPlaceable = true;
        }
    }

    private void MouseDownCanceled()
    {
        mouseDown = false;
        isDraggingPlaceable = false;
    }

    private void MouseDownPerformed()
    {
        if (selectedPlaceable != null) return;

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Placeable placeable = hit.transform.GetComponent<Placeable>();
            if (placeable != null)
                SelectPlacable(placeable);
        }
    }

    private void Update()
    {
        if (isDraggingPlaceable)
        {
            RaycastHit hitFloor = CastRayFromMouseToFloor();
            if (hitFloor.transform != null && selectedPlaceable != null)
            {
                Vector3 position = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.WorldToScreenPoint(selectedPlaceable.transform.position).z);
                Vector3 worldPosition = camera.ScreenToWorldPoint(position);
                selectedPlaceable.transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
            }
        }
        if (selectedPlaceable != null)
        {
            if (selectedPlaceable.IsValidPosition)
            {
                DecorateButtonGroupUIManager.Instance.ShowOkButton();
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                DecorateButtonGroupUIManager.Instance.HideOkButton();
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    public void SelectPlacable(Placeable toBeSelected)
    {
        //deselct previous placeable
        DeselectPlaceable();

        //set new placeable to green
        selectedPlaceable = toBeSelected;
        selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        //enable button group
        DecorateButtonGroupUIManager.Instance.ShowButtonGroup();
        
        //save initial state
        placeableInitialState.position = selectedPlaceable.transform.position;
        placeableInitialState.angle = selectedPlaceable.transform.rotation.y;

        //enable rotation wheel
        selectedPlaceable.GetComponentInChildren<Canvas>().enabled = true;

        //hide inventory
        inventoryScrollView.gameObject.SetActive(false);
    }

    public void DeselectPlaceable(bool savePosition = true)
    {
        if (selectedPlaceable == null) return;

        //if do not save position, put it back to initial state
        if (!savePosition)
        {
            selectedPlaceable.transform.position = placeableInitialState.position;
            selectedPlaceable.RotateToAngle(placeableInitialState.angle);
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            //If position of placeable is not valid, put it back in the inventory
            if (!selectedPlaceable.IsValidPosition)
            {
                RemoveSelectedPlaceable();
                return;
            }
            else
            {
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }
        }
        
        //disable rotation wheel
        selectedPlaceable.GetComponentInChildren<Canvas>().enabled = false;
        selectedPlaceable = null;

        inventoryScrollView.gameObject.SetActive(true);
    }

    private RaycastHit CastRayFromMouseToFloor()
    {
        Vector3 screenMousePosFar = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.nearClipPlane);
        Vector3 worldMousePosFar = camera.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = camera.ScreenToWorldPoint(screenMousePosNear);

        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Floor"));

        return hit;
    }

    public void RemoveSelectedPlaceable()
    {
        if (selectedPlaceable == null) return;

        GameManager.Instance.Inventory.AddItem(selectedPlaceable.PlaceableSO);
        InventoryUIManager.Instance.RepaintInventory();
        Destroy(selectedPlaceable.gameObject);
        selectedPlaceable = null;
        inventoryScrollView.gameObject.SetActive(true);
    }
}
