using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HuntingManager : Singleton<HuntingManager>
{
	[SerializeField] private int maxHealth;
	[SerializeField] private Transform player;
	[SerializeField] private GameObject gameOverUI;
	[SerializeField] private float huntingDurationSeconds;
	[SerializeField] private TextMeshProUGUI huntingTimerText;

	[SerializeField] private Image hurtOverlay;

	[SerializeField] private Image healthFill;
	[SerializeField] private TextMeshProUGUI healthText;

	private int currentHealth;
	private FurnitureInventory huntingInventory;
	private float huntingTimerSeconds;

	public FurnitureInventory HuntingInventory => huntingInventory;

	protected override void Awake()
	{
		base.Awake();

		currentHealth = maxHealth;
		huntingTimerSeconds = huntingDurationSeconds;

		huntingInventory = new FurnitureInventory();
	}

	private void Start()
	{
		huntingTimerText.text = FormatTime(huntingDurationSeconds);
		AudioManager.Instance.Play("Ambience");
	}

	private void Update()
	{
		//health UI
		healthText.text = currentHealth.ToString();
		healthFill.fillAmount = (float)currentHealth / (float)maxHealth;

		if (huntingTimerSeconds > 0)
		{
			huntingTimerSeconds -= Time.deltaTime;
			huntingTimerText.text = FormatTime(huntingTimerSeconds);
		}
		else
		{
			huntingTimerText.text = "00:00";
			RespawnInHouse();
		}
	}

	private void Die()
	{
		// clear the current hunting session's inventory
		huntingInventory.ClearInventory();

		// detach the camera from the player
		Camera camera = player.GetComponentInChildren<Camera>();
		camera.transform.parent = null;

		// destroy all children of the camera
		foreach (Transform child in camera.transform)
			Destroy(child.gameObject);

		// destroy the player object
		Destroy(player.gameObject);

		GameOver();
	}

	// converts seconds to a string in the format "mm:ss"
	private string FormatTime(float seconds)
	{
		int minutes = Mathf.FloorToInt(seconds / 60);
		int remainingSeconds = Mathf.FloorToInt(seconds % 60);
		return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
	}

	public void DealDamageToPlayer(int damage)
	{
		// Set initial alpha to 0.5
		hurtOverlay.color = new Color(hurtOverlay.color.r, hurtOverlay.color.g, hurtOverlay.color.b, 0.5f);

		// Tween the alpha to 0 over the hurtDuration
		LeanTween.alpha(hurtOverlay.rectTransform, 0f, 0.5f).setOnComplete(() =>
		{
			// After the tween completes, ensure the alpha is set to 0
			Color c = hurtOverlay.color;
			hurtOverlay.color = new Color(c.r, c.g, c.b, 0f);
		});

		currentHealth -= damage;
		Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
		if (currentHealth <= 0) Die();
	}

	public void GameOver()
	{
		gameOverUI.SetActive(true);
		GameManager.Instance.ShowCursor();
	}

	public void RespawnInHouse()
	{
		GameManager.Instance.PermanentInventory.MergeInventory(huntingInventory);
		gameOverUI.SetActive(false);
		GameManager.Instance.HideCursor();
		SceneManager.LoadScene(1);
	}
}
