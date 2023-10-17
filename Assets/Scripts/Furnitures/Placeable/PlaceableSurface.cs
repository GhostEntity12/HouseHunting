using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    public bool HasFurnitureOnThisSurface { get; private set; } = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Placeable _))
            HasFurnitureOnThisSurface = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Placeable _))
            HasFurnitureOnThisSurface = false;
    }
}
