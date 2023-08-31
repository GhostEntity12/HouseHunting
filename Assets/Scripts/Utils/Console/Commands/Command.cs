using UnityEngine;

public class Command : ScriptableObject
{
    public string prefix;
    public string tips;

    public Command(string prefix, string tips)
    {
        this.prefix = prefix;
        this.tips = tips;
    }

    protected void Output(string message)
    {
        DeveloperConsole.Instance.SetOutput(message);
    }

    public virtual void Execute(string[] arguments)
    {

    }
}
