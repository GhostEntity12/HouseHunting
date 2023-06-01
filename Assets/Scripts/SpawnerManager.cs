using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
	List<Spawner> spawners = new();

	// This can be swapped with the percentageOfSpawnersToTrigger below if wanted
	[SerializeField] int numToSpawn;

	[Range(0, 1)]
	[SerializeField] float percentageOfSpawnersToTrigger;

	private void Awake()
	{
		spawners = new List<Spawner>(FindObjectsOfType<Spawner>());
		
		numToSpawn = Mathf.FloorToInt(spawners.Count * percentageOfSpawnersToTrigger);

		// Warn if there aren't enough spawners
		//if (spawners.Count < numToSpawn)
		//{
		//	Debug.LogWarning($"There may not be enough spawners - {spawners.Count} spawners were found, a minimum of {numToSpawn} are suggested.");
		//	numToSpawn = spawners.Count;
		//}

		spawners.Shuffle();
		Queue<Spawner> spawnerQueue = new(spawners);
		for (int i = 0; i < numToSpawn; i++)
		{
			if (spawnerQueue.Count == 0)
			{
				Debug.LogWarning("Couldn't spawn the number of requested furniture.");
				break;
			}
			else if (!spawnerQueue.Dequeue().Spawn())
			{
				// Failed spawn
				i--;
			}
		}
	}
}
