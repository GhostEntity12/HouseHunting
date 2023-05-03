using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Placeable : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public bool IsValidPosition { get; private set; } = true;
    public InventoryItem InventoryItem { get; set; }

    private void Awake()
    {
        meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.GetComponent<Placeable>() != null && DecorateInputManager.Instance.SelectedPlaceable == this)
            IsValidPosition = false;
    }

    public void RotateToAngle(float angle)
    {
        meshRenderer.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
