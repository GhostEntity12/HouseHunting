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

        playerInput = new PlayerInput();

        playerInput.General.Jump.performed += ctx => player.Jump();

        playerInput.General.Crouch.performed += ctx => player.SetMoveState(Player.MoveState.Crouch, true);
        playerInput.General.Crouch.canceled += ctx => player.SetMoveState(Player.MoveState.Crouch, false);

        playerInput.General.Run.performed += ctx => player.SetMoveState(Player.MoveState.Sprint, true);
        playerInput.General.Run.canceled += ctx => player.SetMoveState(Player.MoveState.Sprint, false);

        playerInput.General.Interact.performed += ctx => player.Interact();

        playerInput.General.Pause.performed += ctx => PauseMenu.Instance.SetGamePause(true);
        playerInput.General.OpenDevConsole.performed += ctx => DeveloperConsole.Instance.Toggle();
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
}
