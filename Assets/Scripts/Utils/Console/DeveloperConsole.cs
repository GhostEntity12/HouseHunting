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

            string prefix = words[0];

            Command matchCommand = allCommands.Find(c => c.prefix == prefix.ToLower());

            if (matchCommand != null)
                matchCommand.Execute(words.Skip(1).ToArray());
            else
                SetOutput($"{prefix} is not a valid command");

            inputField.text = "";
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    public void Toggle()
    {
        canvas.enabled = !canvas.enabled;
        if (canvas.enabled)
        {
            inputField.ActivateInputField();
            inputField.Select();
            GameManager.Instance.ShowCursor();
            HuntingInputManager.Instance.PlayerInput.Disable();
        } else
        {
            GameManager.Instance.HideCursor();
            HuntingInputManager.Instance.PlayerInput.Enable();
        }
    }

    public void SetOutput(string text)
    {
        outputText.text = text;
    }
}
