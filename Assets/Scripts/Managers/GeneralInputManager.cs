using TMPro;
using UnityEngine;

public class GeneralInputManager : Singleton<GeneralInputManager>
{
    private PlayerInput playerInput;
    private Player player;

    public PlayerInput PlayerInput => playerInput;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();

        if (!player) return;

        playerInput = new PlayerInput();

        playerInput.General.Jump.performed += ctx => player.Jump();

        playerInput.General.Crouch.performed += ctx => player.SetMoveState(Player.MoveState.Crouch, true);
        playerInput.General.Crouch.canceled += ctx => player.SetMoveState(Player.MoveState.Crouch, false);

        // Toggle sprint on keypress only is player is moving
        playerInput.SprintToggle.Run.performed += ctx => CheckHoldingSprint();
        // Need to enable sprint if the player is holding sprint when they start
        playerInput.SprintToggle.Movement.performed += ctx => CheckHoldingSprint();
		// Disable sprint when the player stops moving
		playerInput.SprintToggle.Movement.canceled += ctx => player.SetMoveState(Player.MoveState.Sprint, false);
        
		playerInput.SprintHold.Run.performed += ctx => player.SetMoveState(Player.MoveState.Sprint, true);
        playerInput.SprintHold.Run.canceled += ctx => player.SetMoveState(Player.MoveState.Sprint, false);

		SetSprintModeControls();

        playerInput.General.Jump.performed += ctx => player.Jump();

        playerInput.General.Interact.performed += ctx => player.Interact();

        playerInput.General.Pause.performed += ctx => PauseMenu.Instance.SetGamePause(true);
        playerInput.General.OpenInventory.performed += ctx => InventoryUIManager.Instance.ToggleInventory();
        playerInput.General.OpenDevConsole.performed += ctx => DeveloperConsole.Instance.ToggleDevConsole();
    }

    private void OnEnable()
    {
        playerInput.General.Enable();
    }

    private void FixedUpdate()
    {
        player.Look(playerInput.General.Look.ReadValue<Vector2>());
        player.Move(playerInput.General.Movement.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        playerInput.General.Disable();
    }

    private void CheckHoldingSprint()
    {
        // Only toggle sprint if the player is moving and holding sprint
        if (playerInput.SprintToggle.Run.IsPressed() && playerInput.SprintToggle.Movement.IsPressed())
            player.SetMoveState(Player.MoveState.Sprint, !player.IsSprinting);
    }

    public void SetSprintModeControls()
    {
		if (PlayerPrefs.GetInt("ToggleSprint", 0) == 1)
		{
			playerInput.SprintToggle.Enable();
			playerInput.SprintHold.Disable();
		}
		else
		{
			playerInput.SprintHold.Enable();
			playerInput.SprintToggle.Disable();
		}
	}

    public void SetSprintMode(TMP_Dropdown dropdown)
    {
        PlayerPrefs.SetInt("ToggleSprint", dropdown.value);
        SetSprintModeControls();
    }

	private void OnDestroy() => playerInput.Dispose();
}
