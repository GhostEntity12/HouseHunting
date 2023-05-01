using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float speed = 5.0f;
    private float gravity = -9.81f;
    private Vector3 playerVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
    
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
