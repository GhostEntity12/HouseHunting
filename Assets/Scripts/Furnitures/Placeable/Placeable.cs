using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class Placeable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canPlaceOnSurface;

    private MeshCollider childMeshCollider;

    [field: SerializeField] public MeshRenderer Mesh { get; private set; }

    public bool IsValidPosition { get; private set; } = true;
	public FurnitureItem InventoryItem { get; set; }
    public Material Material { get; set; }
    public MeshCollider ChildMeshCollider => childMeshCollider;
    public bool CanPlaceOnSurface => canPlaceOnSurface;

    private void Start()
    {
        Mesh.transform.localScale *= InventoryItem.scaleFactor;
        Material = DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == InventoryItem.id).materials[InventoryItem.materialIndex];
        Mesh.material = Material;

        childMeshCollider = GetComponentInChildren<MeshCollider>();
    }

    private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        if (other.TryGetComponent(out Placeable _) && HouseManager.Instance.HoldingPlaceable == this)
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

    public void Interact()
    {
        // cannot pick up a placeable if already holding one
        if (HouseManager.Instance.HoldingPlaceable != null) return;

        HouseManager.Instance.HoldingPlaceable = this;

        // when picking up, remove the furniture from the list of house items
        int thisHouseItemIndex = HouseManager.Instance.HouseItems.FindIndex(f => f.position == transform.position);
        HouseManager.Instance.HouseItems.RemoveAt(thisHouseItemIndex);

        childMeshCollider.enabled = false;

        // reset the holding placeable rotation
        Quaternion originalRotation = transform.rotation;
        transform.LookAt(HouseManager.Instance.ExploreCamera.transform.position);
        float rotationDifference = transform.rotation.eulerAngles.y - originalRotation.eulerAngles.y;
        HouseManager.Instance.HoldingPlaceableRotation = -rotationDifference;
    }
}
