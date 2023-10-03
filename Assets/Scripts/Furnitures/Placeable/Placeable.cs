using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Placeable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canPlaceOnSurface;

    private MeshCollider childMeshCollider;

    [field: SerializeField] public MeshRenderer MeshRenderer { get; private set; }
    [field: SerializeField] public MeshFilter MeshFilter { get; private set; }

    public bool IsValidPosition { get; private set; } = true;
	public SaveDataFurniture InventoryItem { get; set; }
    public Material Material { get; set; }
    public MeshCollider ChildMeshCollider => childMeshCollider;
    public bool CanPlaceOnSurface => canPlaceOnSurface;
    public bool Interactable => HouseManager.Instance.HoldingPlaceable == null;
    public string InteractActionText => "Pickup " + InventoryItem.id;


    private void Start()
    {
        childMeshCollider = GetComponentInChildren<MeshCollider>();
    }

    private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        if ((other.TryGetComponent(out Placeable _) && HouseManager.Instance.HoldingPlaceable == this) || other.CompareTag("HouseWall"))
            IsValidPosition = false;
    }

    public void SetTransforms(Vector3 position, float rotation)
    {
        transform.position = position;
        RotateToAngle(rotation);
    }

	public void RotateToAngle(float angle)
    {
        MeshRenderer.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void Interact()
    {
        // cannot pick up a placeable if already holding one
        if (HouseManager.Instance.HoldingPlaceable != null) return;

        // cannot pick up a placeable if something else is on top of it
        PlaceableSurface[] placeableSurfaces = GetComponentsInChildren<PlaceableSurface>();
        foreach (PlaceableSurface placeableSurface in placeableSurfaces)
        {
            if (placeableSurface.HasFurnitureOnThisSurface)
            {
                // TODO: either change this behavior or notify player instead of logging
                Debug.Log("Cannot pickup this furniture because something is on top of it.");
                return;
            }
        }

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
