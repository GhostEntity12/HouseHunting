using UnityEngine;

public class ModelPreview : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] private float secondsPerRotation;

    [SerializeField] private GameObject modelGameObject;

    private void Update()
    {
        float degreesToRotate = 360.0f / secondsPerRotation * Time.deltaTime;
        modelGameObject.transform.Rotate(Vector3.up * degreesToRotate);
    }

    public void ChangeModel(GameObject model)
    {
        Destroy(modelGameObject);
        modelGameObject = Instantiate(model, transform);
    }
}
