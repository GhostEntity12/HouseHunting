using UnityEngine;
using UnityEngine.UI;

public class DecorateButtonGroupUIManager : Singleton<DecorateButtonGroupUIManager>
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Transform buttonGroup;
        
    public void ButtonGroupVisibility(bool visible)
    {
        buttonGroup.gameObject.SetActive(visible);
    }

    public void OkButtonVisibility(bool visible)
    {
        okButton.gameObject.SetActive(visible);
    }
}
