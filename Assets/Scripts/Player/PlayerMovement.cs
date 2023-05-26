using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private SoundAlerter soundAlerter;
    [SerializeField] private float speed = 5.0f;
    private readonly float gravity = -9.81f;
    private Vector3 playerVelocity;
    private float jumpSpeed = 15f;
    public bool isSneaking;
    public bool isSprinting;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        soundAlerter = GetComponent<SoundAlerter>();
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new(input.x, 0, input.y);

        // Slow down the movement if the player is not sprinting
        //float currentSpeed = isSneaking ? speed / 2 : speed;

        float currentSpeed = speed;
        if(isSprinting) {
            currentSpeed = 10f;
        } 
        if (isSneaking) {
            currentSpeed = speed / 2;
        }

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        controller.Move(playerVelocity * Time.deltaTime);

        if (isSneaking)
        {
            soundAlerter.MakeSound(Time.deltaTime * 2, transform.position, 2);
        }
        else
        {
            soundAlerter.MakeSound(Time.deltaTime * 5, transform.position, 5);
        }

    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            float jump = jumpSpeed + gravity;

            playerVelocity.Set(playerVelocity.x, jump, playerVelocity.z);
        }
    }

    public void Crouch(float input)
    {
        if (input > 0) {
            controller.height = 1f;
            Sneak(true);
        }
        else {
            controller.height = 2f;
            Sneak(false);
        }
    }

    public void Sprint(float input)
    {
        if (input > 0) {
            Sprinting(true);
        } 
        else {
            Sprinting(false);
        }
    }

    public void Sneak(bool input)
    {
        isSneaking = input;
    }

    public void Sprinting(bool input)
    {
        isSprinting = input;
    }
}
