using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Command", menuName = "Command/Spawn")]
public class SpawnCommand : Command
{
    public List<Shootable> shootables = new List<Shootable>();

    public SpawnCommand() : base("spawn", "Spawns a shootable. Cannot be called in a non-hunting scene.\nUsage: spawn <furniture id>.\nDefault to first furniture in the data persistence manager list.")
    {
    }

    public override string Help()
    {
        string output = "";

        foreach (Shootable shootable in shootables)
        {
            output += $"\n- {shootable.FurnitureSO.id.ToLower().Replace(" ", "")}";
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
        
        if (shootables.Count == 0)
        {
            Output("Error. No shootable available to spawn.");
            return;
        }

        Shootable shootable;
        if (arguments.Length == 0)
            shootable = shootables[0];
        else
            shootable = shootables.Find(s => s.FurnitureSO.id.ToLower().Replace(" ", "") == arguments[0].ToLower());
                    
        if (shootable == null)
        {
            Output("Error. Invalid shootable id");
            return;
        }
        
        Shootable spawnedShootable = Instantiate(shootable);
        GameObject player = GameManager.Instance.player;
        spawnedShootable.transform.position = player.transform.position + player.transform.forward;
        Output($"{shootable.FurnitureSO.id} spawned at {spawnedShootable.transform.position}");
    }
}
