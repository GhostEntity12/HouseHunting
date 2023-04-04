using UnityEngine;
using UnityEngine.SceneManagement;

public class DecorateInputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Decorate.ExitToHouse.performed += ctx => SceneManager.LoadScene("House");
    }

    void Start()
    {
        Game.Instance.ShowCursor();
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
        Game.Instance.HideCursor();
    }
}
