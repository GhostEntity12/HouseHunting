using UnityEngine;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour, IInteractable
{
	private Transform spawnPoint;

	[SerializeField] private Sprite icon;
	[SerializeField] private string campfireID;

	public (string id, Sprite icon) CampfireInfo => (campfireID, icon);

	private void Awake()
	{
		spawnPoint = transform.GetChild(0);	
	}

	public void SpawnAtCampfire()
	{
		HuntingManager.Instance.Player.GetComponent<PlayerMovement>().Warp(spawnPoint);
		//player.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
	}

	public void Interact()
	{
		SceneManager.LoadScene(1);
	}

}
