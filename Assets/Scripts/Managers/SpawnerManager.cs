using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
	// This can be swapped with the percentageOfSpawnersToTrigger below if wanted
	[Range(0, 1)]
	[SerializeField] float percentageOfSpawnersToTrigger;

	private List<SpawnerBase> spawners = new();

	private void Awake()
	{
		spawners = new List<SpawnerBase>(FindObjectsOfType<SpawnerBase>());

		List<SpawnerBase> guaranteedSpawns = new List<SpawnerBase>();
		
		foreach (SpawnerBase spawner in spawners)
		{
			if (spawner is SpawnerSet set && set.GuaranteedSpawner)
			{
				guaranteedSpawns.Add(set);
			}
		}

		spawners.RemoveAll(s => guaranteedSpawns.Contains(s));
		spawners.Shuffle();
		Queue<SpawnerBase> spawnerQueue = new(spawners);
		Queue<SpawnerBase> guaranteedSpawnerQueue = new(guaranteedSpawns);
		for (int i = 0; i < Mathf.FloorToInt(spawners.Count * percentageOfSpawnersToTrigger);)
		{
			if (spawnerQueue.Count == 0)
			{
				Debug.LogWarning("Couldn't spawn the number of requested furniture.");
				break;
			}
			else
			{
				SpawnerBase spawner = guaranteedSpawns.Count > 0 ? guaranteedSpawnerQueue.Dequeue() : spawnerQueue.Dequeue();
				if (spawner is Spawner || spawner is SpawnerGroup)
					Debug.LogWarning($"{spawner.GetType()} is deprecated, use a SpawnerSet instead!", spawner);
				i += spawner.Spawn() == true ? 1 : 0;
			}
		}
	}
}
