using UnityEngine;
using TMPro;

public class InteractPopupManager : Singleton<InteractPopupManager>
{
    [SerializeField] private TextMeshProUGUI interactText;

    public void SetAction(string actionText)
    {
        interactText.text = $"Press E to {actionText}";
    }
}
