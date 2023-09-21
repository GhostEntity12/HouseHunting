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
	[SerializeField] private float knockbackDrag = 1f;

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
	private Vector3 knockbackForce = new Vector3(0,0,0);
	private List<MoveState> moveState = new List<MoveState>();

	// look
	private float xRotation = 0f;

    private void Awake()
    {
        moveState.Add(MoveState.Walk);
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
			playerVerticalVelocity = 0f;

		movementVector.y = playerVerticalVelocity * Time.deltaTime;

		movementVector += knockbackForce;
		knockbackForce = KnockbackDecay();

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

	public void ApplyKnockback(Vector3 force)
	{
		float knockbackX = knockbackForce.x;
		float knockbackZ = knockbackForce.z;
		knockbackForce = new Vector3(knockbackForce.x + force.x,0,knockbackForce.z + force.z);
		playerVerticalVelocity += force.y;
	}

	private Vector3 KnockbackDecay()
	{
		Vector3 decayedKnockback = Vector3.Lerp(knockbackForce, Vector3.zero, Time.deltaTime * knockbackDrag);
		float knockbackX = decayedKnockback.x;
		float knockbackZ = decayedKnockback.z;

		if (knockbackX > Time.deltaTime * knockbackDrag)
		{
			knockbackX -= Time.deltaTime * knockbackDrag;
		}
		else if (knockbackX < -Time.deltaTime * knockbackDrag)
		{
			knockbackX += Time.deltaTime * knockbackDrag;
		}
		else
		{
			knockbackX = 0f;
		}

		if (knockbackZ > Time.deltaTime * knockbackDrag)
		{
			knockbackZ -= Time.deltaTime * knockbackDrag;
		}
		else if (knockbackZ < -Time.deltaTime * knockbackDrag)
		{
			knockbackZ += Time.deltaTime * knockbackDrag;
		}
		else
		{
			knockbackZ = 0f;
		}

		return new Vector3(knockbackX, 0f, knockbackZ);
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
