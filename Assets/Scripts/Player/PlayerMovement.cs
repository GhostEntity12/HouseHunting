using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float speed = 5.0f;
    private readonly float gravity = -9.81f;
    private Vector3 playerVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new(input.x, 0, input.y);
        controller.Move(speed * Time.deltaTime * transform.TransformDirection(moveDirection));

        playerVelocity.y += gravity * Time.deltaTime;

        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
    
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
