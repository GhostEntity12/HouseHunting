using UnityEngine;
using UnityEngine.UI;

public class DecorateButtonGroupUIManager : Singleton<DecorateButtonGroupUIManager>
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button okButton;
    [SerializeField] private CanvasGroup buttonGroup;
        
    public void ButtonGroupVisibility(bool visible)
    {
        buttonGroup.alpha = visible ? 1 : 0;
        buttonGroup.interactable = buttonGroup.blocksRaycasts = visible;
    }

    public void OkButtonInteractable(bool interactable)
    {
        okButton.interactable = interactable;
    }
}
