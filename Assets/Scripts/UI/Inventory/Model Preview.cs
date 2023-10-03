using UnityEngine;

public class ModelPreview : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] private float secondsPerRotation;

    [SerializeField] private Transform modelHolder;

    private void Update()
    {
        float degreesToRotate = 360.0f / secondsPerRotation * Time.deltaTime;
        modelHolder.Rotate(Vector3.up * degreesToRotate);
    }

    public void SetModel(GameObject newModel)
    {
        if (modelHolder.childCount != 0)
        {
            Destroy(modelHolder.GetChild(0).gameObject);
        }
        Instantiate(newModel, modelHolder);
    }
}
