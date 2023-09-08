using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Deal Damage Command", menuName = "Command/Deal Damage")]
public class DealDamageCommand : Command
{
    public DealDamageCommand() : base("dealdamage", "Deals damage to player. Cannot be called in non-hunting scene. By default deals 1 damage. Pass in a float to specify damage.\nUsage: dealdamage <damage>")
    {
    }

    public override void Execute(string[] arguments)
    {
        if (HuntingManager.Instance == null)
        {
            Output("Cannot be called in non-hunting scene");
            return;
        }

        try
        {
            float damage;
            damage = arguments.Length > 0 ? float.Parse(arguments[0]) : 1f;
            HuntingManager.Instance.DealDamageToPlayer(damage);
            Output($"Dealt {damage} damage to player");
        }
        catch
        {
            Output(arguments[0] + " is not a valid number");
        }
    }
}
