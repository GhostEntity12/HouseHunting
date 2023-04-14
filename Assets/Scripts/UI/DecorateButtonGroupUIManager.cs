using UnityEngine;
using UnityEngine.UI;

public class DecorateButtonGroupUIManager : MonoBehaviour
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Transform buttonGroup;
    private static DecorateButtonGroupUIManager instance;

    public static DecorateButtonGroupUIManager Instance => instance;
    
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }
    
    public void ShowButtonGroup()
    {
        buttonGroup.gameObject.SetActive(true);
    }

    public void HideButtonGroup()
    {
        buttonGroup.gameObject.SetActive(false);
    }

    public void HideOkButton()
    {
        okButton.gameObject.SetActive(false);
    }

    public void ShowOkButton()
    {
        okButton.gameObject.SetActive(true);
    }
}
