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
    private Canvas canvas;

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
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (shootableComponent.IsDead) return;

        //change icon depending on alertness
        if(AI.Alertness == 100 && fill.sprite != exclamationMark)
        {
            AnimateIcon(exclamationMark);
        }
        else if (AI.Alertness != 100)
        {
            ChangeIcon(questionMark);
        }
        
        if(AI.Alertness == 0)
        {
            canvas.enabled = false;
        } else
        {
            canvas.enabled = true;
        }

        // Update the alert icon's fill amount to match the current alertness level
        fill.fillAmount = AI.Alertness / 100f;  // divide by 100 to convert alertness to a percentage

        // Lerp between the low and high alertness colors based on the current alertness level
        fill.color = Color.Lerp(alertnessLowColor, alertnessHighColor, AI.Alertness / 100f);

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
