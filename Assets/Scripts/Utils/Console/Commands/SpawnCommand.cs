using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Command", menuName = "Command/Spawn")]
public class SpawnCommand : Command
{
    public SpawnCommand() : base("spawn", "Spawns a shootable. Cannot be called in a non-hunting scene.\nUsage: spawn <furniture id>.\nDefault to first furniture in the data persistence manager list.")
    {
    }

    public override string Help()
    {
        string output = "";

        foreach (FurnitureSO furniture in DataPersistenceManager.Instance.AllFurnitureSO)
        {
            output += $"\n- {furniture.id.ToLower().Replace(" ", "")}";
        }

        return tips + output;
    }

    public override void Execute(string[] arguments)
    {
        if (HuntingManager.Instance == null) 
        {
            Output("Error. Cannot spawn furniture in this scene.");
            return;
        }

        if (DataPersistenceManager.Instance == null)
        {
            Output("Error. DataPersistenceManager not found in scene.");
            return;
        }
        
        if (DataPersistenceManager.Instance.AllFurnitureSO.Count == 0)
        {
            Output("Error. No furniture to spawn. Check if AllFurnitureSO in DataPersistenceManager is populated.");
            return;
        }

        List<FurnitureSO> allFurnitures = DataPersistenceManager.Instance.AllFurnitureSO;
        Shootable shootable;
        if (arguments.Length == 0)
            shootable = allFurnitures[0].shootablePrefab;
        else
            shootable = allFurnitures.Find(f => f.id.ToLower().Replace(" ", "") == arguments[0].ToLower())?.shootablePrefab;
                    
        if (shootable == null)
        {
            Output("Error. Invalid furniture id");
            return;
        }
        
        Shootable spawnedShootable = Instantiate(shootable);
        Player player = GameManager.Instance.Player;
        spawnedShootable.transform.position = player.transform.position + player.transform.forward;
        Output($"{shootable.FurnitureSO.id} spawned at {spawnedShootable.transform.position}");
    }
}
