using UnityEngine;
using TMPro;

public class HuntingUIManager : Singleton<HuntingUIManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI ammoCounterText;
    public static HuntingUIManager instance;

    private void Awake()
    {
        
    }

    public void SetTimerText(string text)
    {
        timerText.SetText(text);
    }

    public void SetAmmoCounterText(string text)
    {
        ammoCounterText.SetText(text);
    }
}
