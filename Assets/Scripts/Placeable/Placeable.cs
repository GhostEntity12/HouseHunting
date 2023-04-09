using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField] private PlaceableSO placeableSO;

    public bool IsValidPosition { get; private set; } = true;
    public PlaceableSO PlaceableSO => placeableSO;

    private void OnTriggerExit(Collider other) 
    {
        IsValidPosition = true;
    }
    
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.GetComponent<Placeable>() != null && DecorateInputManager.Instance.SelectedPlaceable == this)
            IsValidPosition = false;
    }
}
