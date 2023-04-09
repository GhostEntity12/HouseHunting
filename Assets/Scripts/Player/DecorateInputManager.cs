using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DecorateInputManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private static DecorateInputManager instance;
    private PlayerInput playerInput;
    private Placeable selectedPlaceable = null;
    private bool isDragging = false;
    private Vector3 placeableInitialPosition = Vector3.zero;

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
        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
        if (hit.transform != null && hit.transform.GetComponent<Placeable>() == selectedPlaceable)
        {
            isDragging = true;
        }
    }

    private void MouseDownCanceled()
    {
        isDragging = false;
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
        if (isDragging)
        {
            RaycastHit hitFloor = CastRayFromMouse();
            if (hitFloor.transform != null && selectedPlaceable != null)
            {
                Vector3 position = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), camera.WorldToScreenPoint(selectedPlaceable.transform.position).z);
                Vector3 worldPosition = camera.ScreenToWorldPoint(position);
                selectedPlaceable.transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
                if (selectedPlaceable.IsValidPosition)
                {
                    selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                    DecorateButtonGroupUIManager.Instance.ShowOkButton();
                }
                else
                {
                    selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                    DecorateButtonGroupUIManager.Instance.HideOkButton();
                }
            }
        }
    }

    private void SelectPlacable(Placeable toBeSelected)
    {
        if (selectedPlaceable != null)
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        selectedPlaceable = toBeSelected;
        selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        DecorateButtonGroupUIManager.Instance.ShowButtonGroup();
        placeableInitialPosition = selectedPlaceable.transform.position;
        selectedPlaceable.GetComponentInChildren<Canvas>().enabled = true;
    }

    public void DeselectPlaceable(bool savePosition = true)
    {
        if (selectedPlaceable == null) return;

        if (!savePosition)
        {
            selectedPlaceable.transform.position = placeableInitialPosition;
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        else
        {
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
            }
        }
        selectedPlaceable.GetComponentInChildren<Canvas>().enabled = false;
        selectedPlaceable = null;
        // placeableInitialPosition = Vector3.zero;
    }

    private RaycastHit CastRayFromMouse()
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
