using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected List<Shootable> spawnableFurniture;

    /// <summary>
    /// Attempts to spawn a furniture from its list.
    /// </summary>
    /// <returns>Returns the number of furnitre spawned</returns>
    public virtual int Spawn()
    {
        if (spawnableFurniture.Count == 0)
        {
            Debug.LogError("Spawner is empty!", this);
            return 0;
        }
        Instantiate(spawnableFurniture[Random.Range(0, spawnableFurniture.Count)], transform);
            //Debug.Log($"Spawn location: {transform.position}");
        return 1;
    }
}
