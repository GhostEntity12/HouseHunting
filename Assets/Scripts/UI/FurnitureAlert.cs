using UnityEngine;
using UnityEngine.UI;

public class FurnitureAlert : MonoBehaviour
{
    [SerializeField] private Image outline;
    [SerializeField] private Image fill;
    [SerializeField] private Sprite questionMark;
    [SerializeField] private Sprite exclamationMark;
    [SerializeField] private Sprite skull; // currently using placeholder
    [SerializeField] private Camera mainCamera;

    private WanderAI AI;
    private Shootable shootableComponent;

    private Color alertnessLowColor = Color.green;
    private Color alertnessHighColor = Color.red;

    // Start is called before the first frame update
    private void Awake()
    {
        AI = GetComponentInParent<WanderAI>();
        if (mainCamera == null)
            mainCamera = Camera.main;
        shootableComponent = GetComponentInParent<Shootable>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (shootableComponent.IsDead) return;

        //change icon depending on alertness
        if(AI.alertness == 100 && fill.sprite != exclamationMark)
        {
            AnimateIcon(exclamationMark);
        }
        else if (AI.alertness != 100)
        {
            ChangeIcon(questionMark);
        }

        // Clamp alertness to the range 0-100
        AI.alertness = Mathf.Clamp(AI.alertness, 0, 100);

        // Update the alert icon's fill amount to match the current alertness level
        fill.fillAmount = AI.alertness / 100f;  // divide by 100 to convert alertness to a percentage

        // Lerp between the low and high alertness colors based on the current alertness level
        fill.color = Color.Lerp(alertnessLowColor, alertnessHighColor, AI.alertness / 100f);

        // Make sure the canvas always faces the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void ChangeIcon(Sprite sprite)
    {
        outline.sprite = sprite;
        fill.sprite = sprite;
    }

    private void AnimateIcon(Sprite sprite)
    {
        ChangeIcon(sprite);
        LeanTween.scale(gameObject, new Vector3(1.6f, 1.6f, 1.6f), 0.25f).setEaseOutBounce().setOnComplete(() => {
            LeanTween.scale(gameObject, Vector3.one, 0.25f);
        });
        LeanTween.scale(gameObject, new Vector3(1.6f, 1.6f, 1.6f), 0.25f).setEaseInExpo().setDelay(0.5f).setOnComplete(() => {
            LeanTween.scale(gameObject, Vector3.one, 0.25f);
        });
    }

    public void OnDead()
    {
        ChangeIcon(skull);
        fill.fillAmount = 1;
        fill.color = Color.white;
    }
}
