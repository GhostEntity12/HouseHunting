using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HuntingUIManager : Singleton<HuntingUIManager>
{
    //time
    [SerializeField] private TextMeshProUGUI timerText;

    //ammo
    [SerializeField] private TextMeshProUGUI ammoCounterText;
    [SerializeField] private AmmoUI bulletUI;

    //reload UI
    [SerializeField] private Image reloadBar;
    [SerializeField] private Image reloadFill;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float reloadTime; // Time it takes to reload the gun

    public AmmoUI AmmoUI => bulletUI;

	//public delegate void OnReloadFinish();
	//public static event OnReloadFinish OnReloadFinishEvent;

	//private void Update()
 //   {
 //       if (isReloading)
 //       {
 //           // Increment the timer by the time since the last frame
 //           reloadTimer += Time.deltaTime;

 //           // Calculate the current reload progress as a percentage
 //           float progress = reloadTimer / reloadTime;

 //           // Update the reload bar fill amount
 //           reloadFill.fillAmount = progress;

 //           if (reloadTimer >= reloadTime)
 //           {
 //               // If the gun has finished reloading
 //               isReloading = false;
 //               reloadFill.fillAmount = 0; // Reset the bar to empty
 //               reloadTimer = 0;
 //               reloadBar.gameObject.SetActive(false);
 //               OnReloadFinishEvent?.Invoke();
 //           }
 //       }
 //   }

    protected override void Awake()
    {
        base.Awake();
        reloadBar.gameObject.SetActive(false);
    }

    public void SetTimerText(string text)
    {
        timerText.SetText(text);
    }

    public void SetAmmoCounterText(string text)
    {
        ammoCounterText.SetText(text);
    }

    public void ReloadBarAnimation(float gunReloadTime)
    {
        reloadBar.gameObject.SetActive(true);
        reloadTime = gunReloadTime;
        isReloading = true;
    }
}
