using System.Collections.Generic;
using UnityEngine;

public class SpawnerSet : SpawnerBase
{
	[System.Serializable]
	internal class SpawnBucket
	{
		[SerializeField] private Shootable furniture;
		[SerializeField] private Vector2Int spawnCountRange;

		public void Use(Vector3 pos, float range)
		{
			int numToSpawn = Random.Range(spawnCountRange.x, spawnCountRange.y);
			for (int i = 0; i < numToSpawn; i++)
			{
				Instantiate(furniture, GetRandomSpawnPointInRadius(pos, range), Quaternion.identity);
			}
		}
	}
	[SerializeField] private List<SpawnBucket> guaranteedSpawns = new();
	[SerializeField] private Vector2Int additionalBucketsToUse = new(1, 1);
	[SerializeField] private List<SpawnBucket> additionalSpawns = new();
	[SerializeField] private float range = 5;

	[field: SerializeField] public bool GuaranteedSpawner { get; private set; }
	
	private static Vector3 GetRandomSpawnPointInRadius(Vector3 centerPoint, float radius)
	{
		Vector2 offset2D = Random.insideUnitCircle * radius;
		Vector3 spawnPoint = new(centerPoint.x + offset2D.x, centerPoint.y, centerPoint.z + offset2D.y);
		float height = Terrain.activeTerrain.SampleHeight(spawnPoint);
		return new(spawnPoint.x, height, spawnPoint.z);
	}

	public override bool Spawn()
	{
		// Use guaranteed buckets
		guaranteedSpawns.ForEach(b => b.Use(transform.position, range));

		// Get number of buckets that still need to be used
		int remainingSpawns = Random.Range(additionalBucketsToUse.x, additionalBucketsToUse.y);
		if (remainingSpawns > additionalSpawns.Count)
		{
			Debug.Log("Not enough buckets to fulfil requested number");
			remainingSpawns = additionalSpawns.Count;
		}

		// Use additional buckets
		for (int i = 0; i < remainingSpawns; i++)
		{
			additionalSpawns[i].Use(transform.position, range);
		}
		return true;
	}
}
