using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] CanvasGroup settingsGroup;
    [SerializeField] VideoPlayer animaticPlayer;
    [SerializeField] RawImage animatic;
    [SerializeField] TextMeshProUGUI skipText;

    PlayerInput inputs = new PlayerInput();
	private void Start() 
    {
		AudioManager.Instance.Play("Ambience02");
        (animatic.texture as RenderTexture).Release();
    }

    private void SkipAnimatic(InputAction.CallbackContext ctx)
    {
        if (skipText.enabled)
        {
            inputs.Dispose();
            OnAnimaticEnd(animaticPlayer);
        }
        else
        {
            skipText.enabled = true;
        }
    }

	private void OnAnimaticEnd(VideoPlayer vp)
	{
		animaticPlayer.loopPointReached -= OnAnimaticEnd;
		DataPersistenceManager.Instance.NewGame();
		SceneManager.LoadScene(1);
	}

	public void NewGame()
    {
        animaticPlayer.Play();
        inputs.Animatic.Enable();
        animatic.raycastTarget = true;
        inputs.Animatic.Skip.performed += SkipAnimatic;
        animaticPlayer.loopPointReached += OnAnimaticEnd;
    }

    public void Continue()
    {
        DataPersistenceManager.Instance.LoadGame();
        SceneManager.LoadScene(2);
    }

    public void SetSettingsVisible(bool visible)
    {
        settingsGroup.alpha = visible ? 1 : 0;
        settingsGroup.blocksRaycasts = visible;
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
