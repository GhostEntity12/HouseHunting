using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractPopupManager : Singleton<InteractPopupManager>
{
	[SerializeField] private RectTransform prompt;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private TextMeshProUGUI interactKeyBindingText;
	bool visibility = false;

	private void Start()
	{
		interactKeyBindingText.text = GeneralInputManager.Instance.PlayerInput.General.Interact.GetBindingDisplayString();
	}

	public void SetAction(string actionText)
	{
		interactText.text = actionText;
	}

	public void SetVisibility(bool visible)
	{
		if (visibility != visible)
		{
			visibility = visible;
			if (visible)
			{
				LeanTween.moveY(prompt, 90, 0.2f).setEaseOutBack();
			}
			else
			{
				LeanTween.moveY(prompt, -60, 0.2f).setEaseInBack();
			}
		}
	}
}
