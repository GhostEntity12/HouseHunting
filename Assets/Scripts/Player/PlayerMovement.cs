using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float speed = 5.0f;
    private readonly float gravity = -9.81f;
    private Vector3 playerVelocity;
    public bool isSneaking;
    public bool isMoving;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new(input.x, 0, input.y);

        // Slow down the movement if the player is not sprinting
        float currentSpeed = isSneaking ? speed / 2 : speed;

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        Movement(moveDirection.magnitude);

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

    public void Movement(float input)
    {
        if (input < 0.1)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
}
