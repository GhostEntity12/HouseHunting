using UnityEngine;

public abstract class Command : ScriptableObject
{
    public string prefix;
    [TextArea] public string tips;

    public Command(string prefix, string tips)
    {
        this.prefix = prefix;
        this.tips = tips;
    }

    protected void Output(string message)
    {
        DeveloperConsole.Instance.SetOutput(message);
    }

    public virtual string Help()
    {
        return tips;
    }

    public abstract void Execute(string[] arguments);
}
