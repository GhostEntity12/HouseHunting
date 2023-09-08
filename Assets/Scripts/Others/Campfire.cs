using UnityEngine;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour, IInteractable
{
	private Transform spawnPoint;

	[SerializeField] private Sprite icon;
	[SerializeField] private string campfireID;

	public (string id, Sprite icon) CampfireInfo => (campfireID, icon);

	public string InteractActionText => "Return to House";
	public bool Interactable => true;

    private void Awake()
	{
		spawnPoint = transform.GetChild(0);	
	}

	public void SpawnAtCampfire()
	{
		GameManager.Instance.Player.Warp(spawnPoint);
	}

	public void Interact()
	{
		SceneManager.LoadScene(1);
	}

}
