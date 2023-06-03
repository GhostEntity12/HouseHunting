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
        //Debug.Log(DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == InventoryItem.id));
        Material = DataPersistenceManager.Instance.AllFurnitureSO.Find(x => x.id == InventoryItem.id).materials[InventoryItem.materialIndex];
        Mesh.material = Material;
    }

	private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        // if (other.TryGetComponent(out Placeable _) && HouseInputManager.Instance.SelectedPlaceable == this)
        //     IsValidPosition = false;
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

        GameManager.Instance.PermanentInventory.AddItem(InventoryItem);
        HouseManager.Instance.HoldingPlaceable = this;
        HouseManager.Instance.HoldingPlaceableRotation = 0;
    }
}
