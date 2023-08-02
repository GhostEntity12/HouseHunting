using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<Shootable> spawnableFurniture;
    public int amountToSpawn;

    public bool Spawn()
    {
        if (spawnableFurniture.Count == 0)
        {
            Debug.LogError("Spawner is empty!", this);
            return false;
        }
        for(int i = 0; i < amountToSpawn; i++)
        {
            Instantiate(spawnableFurniture[Random.Range(0, spawnableFurniture.Count)], transform);
            Debug.Log($"Spawn location: {transform.position}");
        }
        return true;
    }
}
