using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeveloperConsole : Singleton<DeveloperConsole>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;

    [SerializeField] private List<Command> allCommands = new List<Command>();

    protected override void Awake()
    {
        base.Awake();

        canvas.enabled = false;

    }

    private void Update()
    {
        if (canvas.enabled && Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {

            string[] words = inputField.text.Split(' ');

            Command matchCommand = allCommands.Find(c => c.prefix == words[0]);

            if (matchCommand != null)
                matchCommand.Execute(words.Skip(1).ToArray());
            else
                SetOutput("Command not found");

            inputField.text = "";
        }
        if (canvas.enabled) inputField.Select();
    }

    public void Toggle()
    {
        canvas.enabled = !canvas.enabled;
        if (canvas.enabled)
        {
            inputField.Select();
            GameManager.Instance.ShowCursor();
        } else
        {
            GameManager.Instance.HideCursor();
        }
    }

    public void SetOutput(string text)
    {
        outputText.text = text;
    }
}
