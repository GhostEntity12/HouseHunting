using UnityEngine;

public class Campfire : MonoBehaviour
{
	private Transform spawnPoint;

	[SerializeField] private string campfireID;

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
