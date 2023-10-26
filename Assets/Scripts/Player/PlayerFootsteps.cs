using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
	private CharacterController controller;
	private bool isWalking;
	private bool input;
	private bool grounded;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		isWalking = false;
		input = false;
	}

	private void Update()
	{
		HandleAudio();
		HandleLanding();
	}

	private void HandleAudio()
	{
		input = HuntingInputManager.Instance && HuntingInputManager.Instance.PlayerInput.General.Movement.ReadValue<Vector2>().magnitude > 0;
		if (controller.isGrounded)
		{
			if (!isWalking && input)
			{
				isWalking = true;
				AudioManager.Instance.Play("WalkOnGrass");
			}
			else if (isWalking && !input)
			{
				isWalking = false;
				AudioManager.Instance.Stop("WalkOnGrass");
			}
		}
		else if (!controller.isGrounded)
		{
			isWalking = false;
			AudioManager.Instance.Pause("WalkOnGrass");
		}
	}

	private void HandleLanding()
	{
		if (grounded != controller.isGrounded)
		{
			grounded = controller.isGrounded;
			if (grounded)
				AudioManager.Instance.Play("Landing");
		}
	}
}
