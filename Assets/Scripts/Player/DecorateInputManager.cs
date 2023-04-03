using UnityEngine;
using UnityEngine.SceneManagement;

public class DecorateInputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        Game.Instance.ShowCursor();

        playerInput = new PlayerInput();

        playerInput.Decorate.ExitToHouse.performed += ctx => SceneManager.LoadScene("House");
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
