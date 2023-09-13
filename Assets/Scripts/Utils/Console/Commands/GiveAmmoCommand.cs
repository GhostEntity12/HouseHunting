using UnityEngine;

[CreateAssetMenu(fileName = "Give Ammo Command", menuName = "Command/Give Ammo")]
public class GiveAmmoCommand : Command
{
    public GiveAmmoCommand() : base("giveammo", "Give ammo to current gun. If argument is passed, will default to 10. Cannot be called in non-hunting scene.\nUsage: giveammo <number>")
    {
    }

    public override void Execute(string[] arguments)
    {
        if (!HuntingManager.Instance)
        {
            Output("Cannot be called in non-hunting scene");
            return;
        }

        try
        {
            int numberToGive = arguments.Length > 0 ? int.Parse(arguments[0]) : 10;
            WeaponManager.Instance.GiveAmmo(numberToGive);
            Output($"Gave {numberToGive} ammo to {WeaponManager.Instance.CurrentGun.GunSO.name}");
        }
        catch
        {
            Output(arguments[0] + " is not a valid number.");
        }
    }
}
