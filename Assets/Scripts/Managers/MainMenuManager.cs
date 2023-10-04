using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] CanvasGroup fade;
    [SerializeField] RectTransform settings;
    [SerializeField] RectTransform credits;
    [SerializeField] VideoPlayer animaticPlayer;
    [SerializeField] RawImage animatic;
    [SerializeField] TextMeshProUGUI skipText;

    private PlayerInput inputs;

    protected override void Awake()
    {
        base.Awake();
        inputs = new PlayerInput();
        SetCreditsVisible(false);
    }

    private void Start() 
    {
		AudioManager.Instance.Play("Ambience02");
        (animatic.texture as RenderTexture).Release();
	}

    private void OnEnable()
    {
        inputs.Animatic.Enable();
    }

    private void OnDisable()
    {
        inputs.Animatic.Disable();
    }

    private void SkipAnimatic(InputAction.CallbackContext ctx)
    {
        if (skipText && skipText.enabled)
        {
            inputs.Dispose();
            OnAnimaticEnd(animaticPlayer);
        }
        else
        {
            skipText.enabled = true;
            inputs.Animatic.SkipInitial.performed -= SkipAnimatic;
            inputs.Animatic.SkipConfirm.performed += SkipAnimatic;

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
        AudioManager.Instance.Pause("Ambience02");
        animaticPlayer.Play();
        inputs.Animatic.Enable();
        animatic.raycastTarget = true;
        inputs.Animatic.SkipInitial.performed += SkipAnimatic;
        animaticPlayer.loopPointReached += OnAnimaticEnd;
    }

    public void Continue()
    {
        DataPersistenceManager.Instance.LoadGame();
        SceneManager.LoadScene(2);
    }

    public void SetSettingsVisible(bool visible)
    {
        if (visible)
        {
		    LeanTween.alphaCanvas(fade, 1, 0.2f);
            LeanTween.moveY(settings, 0, 0.3f).setEaseOutBack();
        }
        else
        {
			LeanTween.alphaCanvas(fade, 0, 0.2f).setDelay(0.2f);
			LeanTween.moveY(settings, 1080, 0.3f).setEaseInBack();
		}
    }
    public void SetCreditsVisible(bool visible)
    {
		if (visible)
		{
			LeanTween.alphaCanvas(fade, 1, 0.2f);
			LeanTween.moveY(credits, 0, 0.3f).setEaseOutBack();
		}
		else
		{
			LeanTween.alphaCanvas(fade, 0, 0.2f).setDelay(0.2f);
			LeanTween.moveY(credits, 1080, 0.3f).setEaseInBack();
		}
	}

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
