using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private float xSensitivity = 20f;
    [SerializeField] private float ySensitivity = 20f;
    private float xRotation = 0f;

    public void Look(Vector2 input)
    {
        xRotation -= (input.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);
    }
}
