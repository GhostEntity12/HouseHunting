using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField] private PlaceableSO placeableSO;

    public PlaceableSO PlaceableSO => placeableSO;
}
