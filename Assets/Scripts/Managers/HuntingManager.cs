using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HuntingManager : Singleton<HuntingManager>
{
	[SerializeField] private int maxHealth;
	[SerializeField] private GameObject gameOverUI;

	[SerializeField] private Image hurtOverlay;

	[SerializeField] private HealthUI healthUI;
	[SerializeField] private Lure lurePrefab;

	private float currentHealth;

	public FurnitureInventory HuntingInventory { get; private set; }
	public int MaxHealth => maxHealth; 
	public Lure LurePrefab => lurePrefab; 
	
	protected override void Awake() 
	{
		base.Awake();

		currentHealth = maxHealth;

		HuntingInventory = new FurnitureInventory();
	}

	private void Start()
	{
		AudioManager.Instance.Play("Ambience");
	}

	private void Die()
	{
		// clear the current hunting session's inventory
		HuntingInventory.ClearInventory();

		// detach the camera from the player
		Camera camera = GameManager.Instance.Player.transform.GetComponentInChildren<Camera>();
		camera.transform.parent = null;

		// destroy all children of the camera
		foreach (Transform child in camera.transform)
			Destroy(child.gameObject);

		// destroy the player object
		Destroy(GameManager.Instance.Player.gameObject);

		GameOver();
	}

	public void DealDamageToPlayer(float damage)
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
		healthUI.SetHealth(currentHealth);
		if (currentHealth <= 0) Die();
	}

	public void GameOver()
	{
		gameOverUI.SetActive(true);
		GameManager.Instance.ShowCursor();
	}

	public void RespawnInHouse()
	{
		GameManager.Instance.PermanentInventory.MergeInventory(HuntingInventory);
		gameOverUI.SetActive(false);
		GameManager.Instance.HideCursor();
		SceneManager.LoadScene(1);
	}
}
