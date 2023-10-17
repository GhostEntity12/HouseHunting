using System;
using UnityEngine;

public class ModelPreview : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] private float secondsPerRotation;

    [SerializeField] private Transform modelHolder;
    [SerializeField] private new Camera camera;

    private float baseFieldOfView;

    private void Awake()
    {
        baseFieldOfView = camera.fieldOfView;
    }

    private void Update()
    {
        float degreesToRotate = 360.0f / secondsPerRotation * Time.deltaTime;
        modelHolder.Rotate(Vector3.up * degreesToRotate);
    }

    public void SetModel(GameObject newModel, float fieldOfViewMultiplier)
    {
        if (modelHolder.childCount != 0)
            Destroy(modelHolder.GetChild(0).gameObject);
        Instantiate(newModel, modelHolder);
        camera.fieldOfView = baseFieldOfView;
        camera.fieldOfView *= Math.Min(fieldOfViewMultiplier, 1);
    }
}
