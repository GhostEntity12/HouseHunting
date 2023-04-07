using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField] private PlaceableSO placeableSO;

    public bool IsValidPosition { get; private set; } = true;
    public PlaceableSO PlaceableSO => placeableSO;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.GetComponent<Placeable>() != null && DecorateInputManager.Instance.SelectedPlaceable == this)
            IsValidPosition = false;
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.GetComponent<Placeable>() != null && DecorateInputManager.Instance.SelectedPlaceable == this)
            IsValidPosition = true;
    }
}
