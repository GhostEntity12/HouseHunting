using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu>
{
	[SerializeField] private CanvasGroup bg;
	[SerializeField] private RectTransform pauseButtons;
	[SerializeField] private RectTransform settings;
	[SerializeField] private GameObject currentView;
	[SerializeField] private TMP_Dropdown sprintType;

	private PlayerInput playerInput;
	private bool isPaused;

	public bool IsPaused => isPaused;

	protected override void Awake()
	{
		base.Awake();
		playerInput = GeneralInputManager.Instance.PlayerInput;

		playerInput.PauseMenu.ResumeGame.performed += ctx => SetGamePause(false);

		sprintType.value = PlayerPrefs.GetInt("ToggleSprint");
		GeneralInputManager.Instance.SetSprintModeControls();
	}

	public void ChangeView(GameObject newView)
	{
		currentView.SetActive(false);
		newView.SetActive(true);
		currentView = newView;
	}

	public void ChangeView(CanvasGroup cGroup)
	{
		bool vis = cGroup.blocksRaycasts;
		cGroup.blocksRaycasts = !vis;
		cGroup.alpha = vis ? 0 : 1;
	}

	public void SetGamePause(bool pause)
	{
		isPaused = pause;

		GeneralInputManager.Instance.enabled = !pause;
		if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = !pause;
		if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = !pause;

		if (pause)
		{
			LeanTween.moveY(pauseButtons, 0, 0.3f).setEaseOutBack().setIgnoreTimeScale(true);
			LeanTween.alphaCanvas(bg, 1, 0.2f).setIgnoreTimeScale(true);
			Time.timeScale = 0;
			playerInput.PauseMenu.Enable();
			GameManager.Instance.ShowCursor();
		}
		else
		{
			// bit of a bodgy fix for resettign the settings...
			// TODO: fix properly
			LeanTween.moveY(pauseButtons, 1080, 0.3f).setEaseInBack().setIgnoreTimeScale(true).setOnComplete(() => {
				ChangeView(pauseButtons.gameObject);
				settings.position = new(Screen.width / 2, Screen.height / 2);
			});
			LeanTween.moveY(settings, 1080, 0.3f).setEaseInBack().setIgnoreTimeScale(true);
			LeanTween.alphaCanvas(bg, 0, 0.2f).setIgnoreTimeScale(true).setDelay(0.2f);
			Time.timeScale = 1;
			playerInput.PauseMenu.Disable();
			GameManager.Instance.HideCursor();
		}
	}
	private void OnDestroy() => playerInput.Dispose();
}
