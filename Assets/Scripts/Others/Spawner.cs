using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<Shootable> spawnableFurniture;

    public bool Spawn()
    {
        if (spawnableFurniture.Count == 0)
        {
            Debug.LogError("Spawner is empty!", this);
            return false;
        }

        Instantiate(spawnableFurniture[Random.Range(0, spawnableFurniture.Count)]);
        return true;
    }
}