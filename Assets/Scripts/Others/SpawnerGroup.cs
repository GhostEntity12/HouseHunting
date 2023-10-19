using UnityEngine;

public class SpawnerGroup : Spawner
{
	[SerializeField] private int numToSpawn = 2;
	[SerializeField] private int range = 5;

	public override int Spawn()
	{
		if (spawnableFurniture.Count == 0)
		{
			Debug.LogError("Spawner is empty!", this);
			return 0;
		}

		Shootable furnitureToSpawn = spawnableFurniture[Random.Range(0, spawnableFurniture.Count)];
		for (int i = 0; i < numToSpawn; i++)
		{
			Vector2 offset2D = Random.insideUnitSphere * range;
			Vector3 spawnPoint = new(transform.position.x + offset2D.x, transform.position.y, transform.position.z + offset2D.y);
			Instantiate(furnitureToSpawn, spawnPoint, Quaternion.identity);
		}

		return numToSpawn;
	}
}
