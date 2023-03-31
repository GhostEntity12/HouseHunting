using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float speed = 5.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        if (!controller.isGrounded)
            controller.Move(Vector3.down * 5 * Time.deltaTime);
    }
}
