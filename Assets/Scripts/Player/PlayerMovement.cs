using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float speed = 5.0f;
	private const float gravity = -9.81f;
	private const float jumpSpeed = 15f;

	private CharacterController controller;

	private float playerVerticalVelocity;
	private float currentSpeed;

	public bool isSneaking;
	public bool isSprinting;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
	}

	public void Move(Vector2 input)
	{
		Vector3 moveDirection = new Vector3(input.x, 0, input.y);

		// Set currentSpeed based on isSneaking and isSprinting flags
		if (isSneaking)
		{
			currentSpeed = speed * 0.5f;
		}
		else if (isSprinting)
		{
			currentSpeed = speed * 1.5f;
		}
		else
		{
			currentSpeed = speed;
		}

		Vector3 movementVector = transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime;

		playerVerticalVelocity += gravity * Time.deltaTime;

		if (controller.isGrounded && playerVerticalVelocity < 0)
			playerVerticalVelocity = -2f;

		movementVector.y = playerVerticalVelocity * Time.deltaTime;

		controller.Move(movementVector);

		if (moveDirection.magnitude > 0)
		{
			// Adjust sound alert levels based on isSneaking and isSprinting flags
			if (isSneaking)
			{
				SoundAlerter.MakeSoundContinuous(2, transform.position, hasFalloff: false);
			}
			else if (isSprinting)
			{
				SoundAlerter.MakeSoundContinuous(8, transform.position, hasFalloff: false);
			}
			else
			{
				SoundAlerter.MakeSoundContinuous(5, transform.position, hasFalloff: false);
			}
		}
	}

	public void Jump()
	{
		if (controller.isGrounded)
		{
			float jump = jumpSpeed + gravity;

			playerVerticalVelocity = jump;

			controller.Move(Vector3.up * playerVerticalVelocity * Time.deltaTime);

			SoundAlerter.MakeSoundImpulse(10, transform.position);
		}
	}

	public void Crouch(float input)
	{
		isSneaking = input > 0;
		controller.height = isSneaking ? 1 : 2;
	}

	public void Run(float input)
	{
		isSprinting = input > 0;
	}

	public void Warp(Transform warpPoint)
	{
		controller.enabled = false;
		transform.SetPositionAndRotation(warpPoint.position, warpPoint.rotation);
		controller.enabled = true;
	}
}
