using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DecorateInputManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private ScrollRect inventoryScrollView;
    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private static DecorateInputManager instance;
    private PlayerInput playerInput;
    private Placeable selectedPlaceable = null;
    private bool isDraggingPlaceable = false;
    private bool isDraggingCamera = false;
    private bool isSelectingPlaceable = false;
    private (Vector3 position, float angle) placeableInitialState = (Vector3.zero, 0f);
    private float cameraYRotation = 0f;
    private (float xMin, float xMax, float zMin, float zMax) cameraBound = (0f, 0f, 0f, 0f);

    public static DecorateInputManager Instance => instance;
    public Placeable SelectedPlaceable => selectedPlaceable;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerInput = new PlayerInput();

        playerInput.Decorate.ExitToHouse.performed += ctx => SceneManager.LoadScene("House");

        playerInput.Decorate.MouseDown.started += ctx => MouseDownStarted();
        playerInput.Decorate.MouseDown.canceled += ctx => MouseDownCanceled();

        playerInput.Decorate.MouseDown.performed += ctx => MouseDownPerformed();

        //camera can only move within 200% of the floor size
        cameraBound.xMin = (floorMeshRenderer.transform.position.x - floorMeshRenderer.bounds.size.x / 2) * 2.0f;
        cameraBound.xMax = (floorMeshRenderer.transform.position.x + floorMeshRenderer.bounds.size.x / 2) * 2.0f;
        cameraBound.zMin = (floorMeshRenderer.transform.position.z - floorMeshRenderer.bounds.size.z / 2) * 2.0f;
        cameraBound.zMax = (floorMeshRenderer.transform.position.z + floorMeshRenderer.bounds.size.z / 2) * 2.0f;
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
        isDraggingPlaceable = false;
        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, int.MaxValue, ~LayerMask.GetMask("Floor"));
        if (hit.transform != null && hit.transform.GetComponent<Placeable>() == selectedPlaceable)
            isDraggingPlaceable = true;
    }

    private void MouseDownCanceled()
    {
        isDraggingCamera = false;
        isDraggingPlaceable = false;
        isSelectingPlaceable = false;
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
        //check if selected placeable is valid position every frame
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
        //check if mouse is over UI, if not, drag camera
        if (Mouse.current.leftButton.wasPressedThisFrame)
            isDraggingCamera = !EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId) && !isDraggingPlaceable && !isSelectingPlaceable;
        if (isDraggingCamera)
        {
            if (selectedPlaceable != null)
                if (selectedPlaceable.GetComponentInChildren<RotationWheel>().IsRotating) return;
            RotateCamera(playerInput.Decorate.MouseMove.ReadValue<Vector2>());
        }
    }

    private void FixedUpdate() 
    {
        MoveCamera(playerInput.Decorate.MoveCamera.ReadValue<Vector2>());
    }

    private void MoveCamera(Vector2 input)
    {
        Vector3 direction = camera.transform.forward * input.y + camera.transform.right * input.x;
        direction.y = 0;

        camera.transform.position = new Vector3(
            Mathf.Clamp(camera.transform.position.x + direction.x * cameraSpeed * Time.deltaTime, cameraBound.xMin, cameraBound.xMax),
            camera.transform.position.y,
            Mathf.Clamp(camera.transform.position.z + direction.z * cameraSpeed * Time.deltaTime, cameraBound.zMin, cameraBound.zMax)
        );
    }

    public void SelectPlacable(Placeable toBeSelected)
    {
        isSelectingPlaceable = true;

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

    private void RotateCamera(Vector2 mouseDelta)
    {
        cameraYRotation -= mouseDelta.x * Time.deltaTime * 30;

        camera.transform.localRotation = Quaternion.Euler(camera.transform.localRotation.eulerAngles.x, cameraYRotation, 0);
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
