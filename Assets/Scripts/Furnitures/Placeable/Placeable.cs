using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Placeable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public MeshRenderer Mesh { get; private set; }
    [field: SerializeField] public RotationWheel RotationWheel { get; private set; }

    public bool IsValidPosition { get; private set; } = true;
	public FurnitureItem InventoryItem { get; set; }
    public Material Material { get; set; }

    private void Start()
    {
        Mesh.transform.localScale *= InventoryItem.scaleFactor;
        Material = DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == InventoryItem.id).materials[InventoryItem.materialIndex];
        Mesh.material = Material;
    }

    private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        Debug.Log(other.name);
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

        // find mesh collider in children and change to is trigger to prevent furniture from moving the player when positioning it
        MeshCollider meshCollider = GetComponentInChildren<MeshCollider>();
        meshCollider.enabled = false;
        //meshCollider.convex= true;
        //meshCollider.isTrigger= true;

        // reset the holding placeable rotation
        HouseManager.Instance.HoldingPlaceableRotation = 0;
    }
}
