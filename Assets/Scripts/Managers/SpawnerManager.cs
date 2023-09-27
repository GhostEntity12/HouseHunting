using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
	// This can be swapped with the percentageOfSpawnersToTrigger below if wanted
	[Range(0, 1)]
	[SerializeField] float percentageOfSpawnersToTrigger;

	private List<Spawner> spawners = new();

	private void Awake()
	{
		spawners = new List<Spawner>(FindObjectsOfType<Spawner>());
		spawners.AddRange(new List<SpawnerGroup>(FindObjectsOfType<SpawnerGroup>()));

		spawners.Shuffle();
		Queue<Spawner> spawnerQueue = new(spawners);
		for (int i = 0; i < Mathf.FloorToInt(spawners.Count * percentageOfSpawnersToTrigger);)
		{
			if (spawnerQueue.Count == 0)
			{
				Debug.LogWarning("Couldn't spawn the number of requested furniture.");
				break;
			}
			else
			{
				i += spawnerQueue.Dequeue().Spawn();
			}
		}
	}
}
