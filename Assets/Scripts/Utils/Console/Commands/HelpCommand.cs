using UnityEngine;

[CreateAssetMenu(fileName = "Help Command", menuName = "Command/Help")]
public class HelpCommand : Command
{
    public HelpCommand() : base("help", "Displays the command description. Pass in 0 arguments to see list of commands")
    {
    }

    public override void Execute(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            string output = "";
            for (int i = 0; i < DeveloperConsole.Instance.AllCommands.Count; i++)
            {
                output += $"{i + 1} - {DeveloperConsole.Instance.AllCommands[i].prefix}\n";
            }
            Output(output);
        }
        else
        {
            Command match = DeveloperConsole.Instance.AllCommands.Find(c => c.prefix.ToLower() == arguments[0].ToLower());
            if (match != null)
                Output(match.Help());
            else
                Output($"Error. Could not find {arguments[0]} in the list of commands.");
        }
    }
}
