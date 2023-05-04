using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float speed = 5.0f;
    private float gravity = -9.81f;
    private Vector3 playerVelocity;
    public bool isSneaking;
    private void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // Slow down the movement if the player is not sprinting
        float currentSpeed = isSneaking ? speed / 2 : speed;

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        controller.Move(playerVelocity * Time.deltaTime);
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

    public void Sneak(bool input)
    {
        isSneaking = input;
    }
}
