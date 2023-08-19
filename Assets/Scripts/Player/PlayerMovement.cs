using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private const float gravity = -9.81f;
    private const float jumpSpeed = 15f;

    private CharacterController controller;
    private SoundAlerter soundAlerter;

    private Vector3 playerVelocity;
    private float currentSpeed;

    public bool isSneaking;
    public bool isSprinting;
    public bool isJumping;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        soundAlerter = GetComponent<SoundAlerter>();
    }
    public void Update()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
            isJumping = false;
        }
    }

    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        // Set currentSpeed based on isSneaking and isSprinting flags
        if (isSneaking) {
            currentSpeed = speed / 2;
        } else if (isSprinting) {
            currentSpeed = speed * 1.5f;
             Debug.Log("isSprinting");
        } else {
            currentSpeed = speed;
        }

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        
        controller.Move(playerVelocity * Time.deltaTime);

        // Adjust sound alert levels based on isSneaking and isSprinting flags
        if (isSneaking) {
            soundAlerter.MakeSound(Time.deltaTime * 2, transform.position, 2);
        } else if (isSprinting){
            soundAlerter.MakeSound(Time.deltaTime * 8, transform.position, 8);
        } else {
            soundAlerter.MakeSound(Time.deltaTime * 5, transform.position, 5);
        }
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            float jump = jumpSpeed + gravity;

            playerVelocity.Set(playerVelocity.x, jump, playerVelocity.z);

            controller.Move(playerVelocity * Time.deltaTime);

            soundAlerter.MakeSound(1, transform.position, 1);

            isJumping = true;
        }
    }

    public void Crouch(float input)
    {
        if (input > 0)
        {
            controller.height = 1f;
            Sneak(true);
        }
        else
        {
            controller.height = 2f;
            Sneak(false);
        }
    }

    public void Run(float input)
    {
        if (input > 0) {
            Sprint(true);
            Debug.Log("Run");
            
        }
        else {
            Sprint(false);
        }
    }
    public void Sneak(bool input)
    {
        isSneaking = input;
    }

    public void Sprint(bool input)
    {
        isSprinting = input;
        Debug.Log("Sprint");
    }
}
