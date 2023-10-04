using UnityEngine;
using UnityEngine.UI;

public class FurnitureAlert : MonoBehaviour
{
	[SerializeField] private Image outline;
	[SerializeField] private Image fill;
	[SerializeField] private Sprite questionMark;
	[SerializeField] private Sprite exclamationMark;
	[SerializeField] private Sprite skull; 
	[SerializeField] Gradient colorLerpGradient;

	private Canvas canvas;
	private Camera mainCamera;
	private WanderAI AI;
	private Shootable shootableComponent;
	private float percentage = 0;


	// Start is called before the first frame update
	private void Awake()
	{
		AI = GetComponentInParent<WanderAI>();
		shootableComponent = GetComponentInParent<Shootable>();
		canvas = GetComponent<Canvas>();
		mainCamera = Camera.main;
	}

	// Update is called once per frame
	private void Update()
	{
		if (shootableComponent.IsDead) return;

		// divide by 100 to convert alertness to a percentage
		percentage = AI.Alertness / 100f;

		canvas.enabled = percentage != 0;

		// Update the alert icon's fill amount to match the current alertness level
		fill.fillAmount = percentage;

		// Lerp between the low and high alertness colors based on the current alertness level
		fill.color = colorLerpGradient.Evaluate(percentage);

		//change icon depending on alertness
		if (percentage == 1 && fill.sprite != exclamationMark)
		{
			// Reached 100, set sprite
			AnimateIcon(exclamationMark);
		}
		else if (percentage != 1 && fill.sprite == exclamationMark)
		{
			// Dropped below 100
			SetIcon(questionMark);
		}

		// Make sure the canvas always faces the camera
		transform.rotation = mainCamera.transform.rotation;
	}

	public void SetIcon(Sprite sprite)
	{
		outline.sprite = sprite;
		fill.sprite = sprite;
	}

	private void AnimateIcon(Sprite sprite)
	{
		SetIcon(sprite);
		LeanTween.scale(gameObject, new Vector3(1.6f, 1.6f, 1.6f), 0.25f).setEaseOutBounce().setOnComplete(() =>
		{
			LeanTween.scale(gameObject, Vector3.one, 0.25f);
		});
		LeanTween.scale(gameObject, new Vector3(1.6f, 1.6f, 1.6f), 0.25f).setEaseInExpo().setDelay(0.5f).setOnComplete(() =>
		{
			LeanTween.scale(gameObject, Vector3.one, 0.25f);
		});
	}

	public void SetDead()
	{
		SetIcon(skull);
		fill.fillAmount = 0;
		canvas.enabled = true;
	}
}
