using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Placeable : MonoBehaviour
{
    [field: SerializeField] public MeshRenderer Mesh { get; private set; }
    [field: SerializeField] public RotationWheel RotationWheel { get; private set; }

    public bool IsValidPosition { get; private set; } = true;
	public InventoryItem InventoryItem { get; set; }

    private void Start()
    {
        Mesh.transform.localScale *= InventoryItem.scaleFactor;
    }

	private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        if (other.TryGetComponent(out Placeable _) && HouseInputManager.Instance.SelectedPlaceable == this)
            IsValidPosition = false;
    }

    public void SetTransforms(Vector3 position, float rotation)
    {
        transform.position = position;
        RotateToAngle(rotation);
    }

	public void RotateToAngle(float angle)
    {
        Mesh.transform.rotation = Quaternion.Euler(0, angle, 0);
    }


}
