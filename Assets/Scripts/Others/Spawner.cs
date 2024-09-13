using System.Collections.Generic;
using UnityEngine;

public class Spawner : SpawnerBase
{
    [SerializeField] protected List<Shootable> spawnableFurniture;

    /// <summary>
    /// Attempts to spawn a furniture from its list.
    /// </summary>
    /// <returns>Returns the number of furnitre spawned</returns>
    public override bool Spawn()
    {
        if (spawnableFurniture.Count == 0)
        {
            Debug.LogError("Spawner is empty!", this);
            return false;
        }
        Instantiate(spawnableFurniture[Random.Range(0, spawnableFurniture.Count)], transform);
        return true;
    }
}
