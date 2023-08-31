using TMPro;
using UnityEngine;

public class DeveloperConsole : Singleton<DeveloperConsole>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputField;

    protected override void Awake()
    {
        base.Awake();

        canvas.enabled = false;
    }

    private void Update()
    {
        if (canvas.enabled && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(inputField.text + " executed");
            inputField.text = "";
            inputField.Select();
        }
    }

    public void Toggle()
    {
        canvas.enabled = !canvas.enabled;
        if (canvas.enabled) inputField.Select();
    }
}
