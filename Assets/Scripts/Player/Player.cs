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
	private float playerVerticalVelocity;
	private Vector3 knockbackForce = new Vector3(0,0,0);
	private List<MoveState> moveState = new List<MoveState>();
	public bool IsSprinting => moveState.Count > 0 && moveState[^1] == MoveState.Sprint;

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

		movementVector += knockbackForce;
		knockbackForce = KnockbackDecay();

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

	public void ApplyKnockback(Vector3 force)
	{
		knockbackForce = new Vector3(knockbackForce.x + force.x,0,knockbackForce.z + force.z);
		playerVerticalVelocity += force.y;
	}
	/// <summary>
	///	Knockback Decay
	///	<para>This function does two things to directly ease knockbackForce to zero.</para>
	///	<para>The first thing it does is apply linear interpolation to zero using deltaTime and knockbackDrag</para>
	///	<para>The second thing it does is either add or subtract deltaTime * knockbackDrag for X and Z axis, until they reach 0.</para>
	///	<para>The if statements are used to specifically decide whether the velocity of a specific direction needs to be incremented or decremented to eventually reach 0/para>
	/// </summary>
	private Vector3 KnockbackDecay()
	{
		// Smooth means of slowly reducing the knockbackforce
		Vector3 decayedKnockback = Vector3.Lerp(knockbackForce, Vector3.zero, Time.deltaTime * knockbackDrag);
		// Another set of knockback decay that has the most effect when the knockback velocity is low, and sets it to 0 when low enough.
		// For both axis they're either added or subtracted by deltatime * drag, depending on whether the number is positive or negative.
		// The main point of these statements is to allow the knockback velocity to slow to a stop and not
		if (decayedKnockback.x > Time.deltaTime * knockbackDrag)
		{
			decayedKnockback.x -= Time.deltaTime * knockbackDrag;
		}
		else if (decayedKnockback.x < -Time.deltaTime * knockbackDrag)
		{
			decayedKnockback.x += Time.deltaTime * knockbackDrag;
		}
		else
		{
			decayedKnockback.x = 0f;
		}

		if (decayedKnockback.z > Time.deltaTime * knockbackDrag)
		{
			decayedKnockback.z -= Time.deltaTime * knockbackDrag;
		}
		else if (decayedKnockback.z < -Time.deltaTime * knockbackDrag)
		{
			decayedKnockback.z += Time.deltaTime * knockbackDrag;
		}
		else
		{
			decayedKnockback.z = 0f;
		}

		return new Vector3(decayedKnockback.x, 0f, decayedKnockback.z);
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
