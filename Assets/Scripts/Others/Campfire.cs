using UnityEngine;

public class Campfire : MonoBehaviour
{
	[SerializeField] private string campfireID;

	private Transform spawnPoint;

	public string CampfireID => campfireID;

    private void Awake()
	{
		spawnPoint = transform.GetChild(0);	
	}

	public void SpawnAtCampfire()
	{
		GameManager.Instance.Player.Warp(spawnPoint);
	}
}
