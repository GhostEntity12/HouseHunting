using System.Collections.Generic;
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
	private float playerVerticalVelocity;
	private List<MoveState> moveState = new List<MoveState>();

	// look
	private float xRotation = 0f;

	public Vector3 camOffset { get; private set; }

	[Header("Sounds")]
	[SerializeField] SoundAlertSO moveSoundCrouch;
	[SerializeField] SoundAlertSO moveSoundWalk;
	[SerializeField] SoundAlertSO moveSoundSprint;
	[SerializeField] SoundAlertSO jumpSound;

    private void Awake()
    {
        moveState.Add(MoveState.Walk);
		camOffset = Camera.main.transform.localPosition;

	}

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactRange) && hit.transform.TryGetComponent(out IInteractable interactable) && interactable.Interactable)
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
            SoundAlerter.MakeSound(moveState[^1] switch
			{
				MoveState.Crouch => moveSoundCrouch,
				MoveState.Walk => moveSoundWalk,
				MoveState.Sprint => moveSoundSprint,
			}, transform.position);
	}

	public void Jump()
	{
		if (controller.isGrounded)
		{
			float jump = jumpSpeed + gravity;

			playerVerticalVelocity = jump;

			controller.Move(playerVerticalVelocity * Time.deltaTime * Vector3.up);

			SoundAlerter.MakeSound(jumpSound, transform.position);
		}
	}

	/// <summary>
	///	Set Move State
	///	<para>Move state is a list filled with MoveState, the first on the list will always be walk, and cannot be removed.</para>
	///	<para>When this method is called, it will push/remove the movementState passed in to the list</para>
	///	<para>The move state will always be the last element on the list</para>
	///	<para>This is so that if the player presses crtl then shift, then releases one of them, the move state will not get set to walking, instead it will be changed to whatever key the player is holding down</para>
	/// </summary>
	/// <param name="movementState">Which movementState to add or remove</param>
	/// <param name="add">true if add, false if remove</param>
	public void SetMoveState(MoveState movementState, bool add)
	{
		if (add)
			moveState.Add(movementState);
		else
			moveState.Remove(movementState);

		MoveState currentState = moveState[moveState.Count - 1];

		switch (currentState)
		{
			case MoveState.Crouch:
				controller.height = 1;
				speedMultiplyer = 0.5f;
				break;
			case MoveState.Walk:
				controller.height = 2;
				speedMultiplyer = 1f;
				break;
			case MoveState.Sprint:
				controller.height = 2;
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
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, interactRange) && hit.transform.TryGetComponent(out IInteractable interactable))
            interactable.Interact();
    }
}
