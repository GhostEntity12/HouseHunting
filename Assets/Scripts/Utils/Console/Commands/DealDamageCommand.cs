using UnityEngine;

[CreateAssetMenu(fileName = "Deal Damage Command", menuName = "Command/Deal Damage")]
public class DealDamageCommand : Command
{
    public DealDamageCommand() : base("dealdamage", "Deals 1 damage to player. Cannot be called in non-hunting scene")
    {
    }

    public override void Execute(string[] arguments)
    {
        if (HuntingManager.Instance == null)
        {
            Output("Cannot be called in non-hunting scene");
            return;
        }

        HuntingManager.Instance.DealDamageToPlayer(1);
        Output("Dealt 1 damage to player");
    }
}
