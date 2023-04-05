using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DecorateInputManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private PlayerInput playerInput;
    private bool mouseDown = false;
    private Placeable selectedPlaceable = null;

    void Awake()
    {
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
                // If the object we hit is a placeable, ignore collisions with it
                if (hit.transform.GetComponent<Placeable>() != null)
                {
                    Physics.IgnoreCollision(selectedPlaceable.GetComponent<Collider>(), hit.transform.GetComponent<Collider>());
                }
                else
                    selectedPlaceable.transform.position = new Vector3(hit.point.x, selectedPlaceable.transform.position.y, hit.point.z);
            }
        }
    }

    private void SelectPlacable(Placeable placeable)
    {
        if (placeable != null)
        {
            if (selectedPlaceable != null)
                selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            selectedPlaceable = placeable;
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            selectedPlaceable.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            selectedPlaceable = null;
        }
    }
}
