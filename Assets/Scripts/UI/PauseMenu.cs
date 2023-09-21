using UnityEngine;

public class PauseMenu : Singleton<PauseMenu>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject currentView;

    private PlayerInput playerInput;
    private bool isPaused;

    public bool IsPaused => isPaused;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GeneralInputManager.Instance.PlayerInput;

        playerInput.PauseMenu.ResumeGame.performed += ctx => SetGamePause(false);
        canvas.enabled = false;
    }

    public void ChangeView(GameObject newView)
    {
        currentView.SetActive(false);
        newView.SetActive(true);
        currentView = newView;
    }

    public void SetGamePause(bool pause)
    {
        isPaused = pause;
        canvas.enabled = pause;

        GeneralInputManager.Instance.enabled = !pause;
        if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = !pause;
        if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = !pause;

        if (pause)
        {
            Time.timeScale = 0;
            playerInput.PauseMenu.Enable();
            GameManager.Instance.ShowCursor();
        }
        else
        {
            Time.timeScale = 1;
            playerInput.PauseMenu.Disable();
            GameManager.Instance.HideCursor();
        }
    }    
}
