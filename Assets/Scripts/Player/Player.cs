using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public enum MoveState { Crouch, Walk, Sprint };

	[Header("Movement")]
	[SerializeField] private CharacterController controller;
	[SerializeField] private float baseSpeed = 5f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpSpeed = 15f;

	[Header("Look")]
	[SerializeField] private new Camera camera;
	[SerializeField] private float xSensitivity = 20f;
	[SerializeField] private float ySensitivity = 20f;

	[Header("Interact")]
	[SerializeField] private float interactRange = 3f;

	// movement
	private float speedMultiplyer = 1f;
	private float moveVolume = 6f;
	private float playerVerticalVelocity;

	// look
	private float xRotation = 0f;

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.transform.TryGetComponent(out IInteractable interactable) && interactable.Interactable)
        {
			InteractPopupManager.Instance.gameObject.SetActive(true);
			InteractPopupManager.Instance.SetAction(interactable.InteractActionText);
        } 
		else
		{
			InteractPopupManager.Instance.gameObject.SetActive(false);
		}
    }

    #region Movement
    public void Move(Vector2 input)
	{
		Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        Vector3 movementVector = baseSpeed * speedMultiplyer * Time.deltaTime * transform.TransformDirection(moveDirection);

		playerVerticalVelocity += gravity * Time.deltaTime;

		if (controller.isGrounded && playerVerticalVelocity < 0)
			playerVerticalVelocity = -2f;

		movementVector.y = playerVerticalVelocity * Time.deltaTime;

		controller.Move(movementVector);

		if (moveDirection.magnitude > 0)
            SoundAlerter.MakeSoundContinuous(moveVolume, transform.position, hasFalloff: false);
	}

	public void Jump()
	{
		if (controller.isGrounded)
		{
			float jump = jumpSpeed + gravity;

			playerVerticalVelocity = jump;

			controller.Move(playerVerticalVelocity * Time.deltaTime * Vector3.up);

			SoundAlerter.MakeSoundImpulse(10, transform.position);
		}
	}

	public void SetMoveState(MoveState movementState)
	{
		switch (movementState)
		{
			case MoveState.Crouch:
				controller.height = 1;
				moveVolume = 2;
				speedMultiplyer = 0.5f;
				break;
			case MoveState.Walk:
				controller.height = 2;
				moveVolume = 6;
				speedMultiplyer = 1f;
				break;
			case MoveState.Sprint:
				controller.height = 2;
				moveVolume = 8;
				speedMultiplyer = 1.5f;
				break;
			default:
				throw new System.Exception("Invalid movement state");
		}
	}

	public void Warp(Transform warpPoint)
	{
		controller.enabled = false;
		transform.SetPositionAndRotation(warpPoint.position, warpPoint.rotation);
		controller.enabled = true;
	}
    #endregion

    #region Look
    public void Look(Vector2 input)
    {
        xRotation -= (input.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);
    }

    public void ChangeSensitivity(Slider slider)
    {
        xSensitivity = slider.value;
        ySensitivity = slider.value;
    }
    #endregion

	public void Interact()
	{
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 3f) && hit.transform.TryGetComponent(out IInteractable interactable))
            interactable.Interact();
    }
}
