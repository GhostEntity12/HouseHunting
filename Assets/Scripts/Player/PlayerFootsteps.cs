using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private CharacterController controller;
    private bool isWalking;
    private bool keysPressed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        isWalking = false;
        keysPressed = false;
    }

    void Update()
    {
        HandleFootsteps();
        HandleAudio();
    }

    private void HandleFootsteps()
    {
        keysPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    private void HandleAudio()
    {
        if (controller.isGrounded)
        {
            if (!isWalking && keysPressed)
            {
                isWalking = true;
                AudioManager.Instance.Play("WalkOnGrass");
            }
            else if (isWalking && !keysPressed)
            {
                isWalking = false;
                AudioManager.Instance.Stop("WalkOnGrass");
            }
        }
        else if (!controller.isGrounded)
        {
            isWalking = false;
            AudioManager.Instance.Pause("WalkOnGrass");
        }
    }
}
