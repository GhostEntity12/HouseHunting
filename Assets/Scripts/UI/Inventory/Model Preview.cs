using UnityEngine;

public class ModelPreview : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] private float secondsPerRotation;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    public MeshFilter MeshFilter => meshFilter;
    public MeshRenderer MeshRenderer => meshRenderer;

    private void Update()
    {
        float degreesToRotate = 360.0f / secondsPerRotation * Time.deltaTime;
        meshFilter.transform.Rotate(Vector3.up * degreesToRotate);
    }
}