using UnityEngine;
[CreateAssetMenu(fileName = "Weapon Wheel Command", menuName = "Command/Weapon Wheel")]
public class WeaponWheelCommand : Command
{
	public WeaponWheelCommand() : base("weaponwheel", "Set whether the weapon wheel is able to be accessed. Defaults to false\nUsage: weaponwheel [true|false]")
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
			if (arguments.Length == 0)
			{
				HuntingInputManager.Instance.WeaponWheelActive(false);
			}
			else
			{
				string arg = arguments[0].ToLowerInvariant();
				if (arg == "true")
				{
					HuntingInputManager.Instance.WeaponWheelActive(true);
					Output($"Enabled weaponwheel");
				}
				else if (arg == "false")
				{
					HuntingInputManager.Instance.WeaponWheelActive(false);
					Output($"Disabled weaponwheel");
				}
				else
				{
					Output($"Invalid argument {arg}");
				}
			}
			
		}
		catch
		{
			Output(arguments[0] + " is not a valid number.");
		}
	}
}
