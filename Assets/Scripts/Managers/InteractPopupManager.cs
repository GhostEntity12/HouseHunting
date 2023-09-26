using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractPopupManager : Singleton<InteractPopupManager>
{
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactKeyBindingText;

    private void Start()
    {
        interactKeyBindingText.text = GeneralInputManager.Instance.PlayerInput.General.Interact.GetBindingDisplayString();
    }

    public void SetAction(string actionText)
    {
        interactText.text = actionText;
    }
}
