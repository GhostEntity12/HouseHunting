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

    private PlayerInput playerInput;

    public List<Command> AllCommands => allCommands;

    protected override void Awake()
    {
        base.Awake();

        canvas.enabled = false;

        playerInput = new PlayerInput();

        playerInput.DevConsole.CloseDevConsole.performed += ctx => Toggle();
        playerInput.DevConsole.Submit.performed += ctx => ExecuteCommand();
    }

    private void ExecuteCommand()
    {
        if (!canvas.enabled || inputField.text == string.Empty) return;

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

    public void Toggle()
    {
        canvas.enabled = !canvas.enabled;
        // disable input when console is open, enable input when console is closed
        GeneralInputManager.Instance.enabled = !canvas.enabled;
        if (HuntingInputManager.Instance) HuntingInputManager.Instance.enabled = !canvas.enabled;
        else if (HouseInputManager.Instance) HouseInputManager.Instance.enabled = !canvas.enabled;

        if (canvas.enabled)
        {
            inputField.ActivateInputField();
            inputField.Select();
            playerInput.DevConsole.Enable();
            GameManager.Instance.ShowCursor();
        } 
        else
        {
            inputField.DeactivateInputField();
            inputField.text = "";
            playerInput.DevConsole.Disable();
            GameManager.Instance.HideCursor();
        }
    }

    public void SetOutput(string text)
    {
        outputText.text = text;
    }
}
