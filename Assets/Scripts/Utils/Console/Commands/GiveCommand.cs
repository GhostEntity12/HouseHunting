using UnityEngine;

[CreateAssetMenu(fileName = "Give Command", menuName = "Command/Give")]
public class GiveCommand : Command
{
    public GiveCommand() : base("give", "Gives an inventory item, can be called in house or hunting scene.\nUsage: give <furniture id>\nDefault to first furniture in the data persistence manager list.\nUse give all to give one of every furniture.")
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
        if (DataPersistenceManager.Instance == null)
        {
            Output("Error. DataPersistenceManager not found in scene.");
            return;
        }

        if (DataPersistenceManager.Instance.AllFurnitureSO.Count == 0)
        {
            Output("Error. No furniture to give. Check if AllFurnitureSO in DataPersistenceManager is populated.");
            return;
        }

        FurnitureSO furniture;
        FurnitureInventory inventory;

        inventory = HuntingManager.Instance ? HuntingManager.Instance.HuntingInventory : GameManager.Instance.PermanentInventory;

        if (arguments.Length == 0)
        {
             furniture = DataPersistenceManager.Instance.AllFurnitureSO[0];
        }
        else if (arguments[0].ToLower() == "all")
        {
            foreach (FurnitureSO item in DataPersistenceManager.Instance.AllFurnitureSO)
            {
                Output($"Added {item.name} into inventory");
                inventory.AddItem(new SaveDataFurniture(item.id, 1, 0, item.basePrice));
            }


            return;
        }
        else
        {
            furniture = DataPersistenceManager.Instance.AllFurnitureSO.Find(f => f.id.ToLower().Replace(" ", "") == arguments[0].ToLower());
        }
                    
        if (furniture == null)
        {
            Output("Error. Invalid furniture id");
            return;
        }

        if (HuntingManager.Instance != null)
        {
            HuntingManager.Instance.HuntingInventory.AddItem(new SaveDataFurniture(furniture.id, 1, 0, furniture.basePrice));
            Output($"{furniture.name} has been added to hunting inventory.");
        }
        else if (HouseManager.Instance != null)
        {
            GameManager.Instance.PermanentInventory.AddItem(new SaveDataFurniture(furniture.id, 1, 0, furniture.basePrice));
            Output($"{furniture.name} has been added to permanent inventory.");
        }
    }
}
