using UnityEngine;

[CreateAssetMenu(fileName = "Placeable_Furniture", menuName = "Furniture/Placecable")]
public class PlaceableSO : ScriptableObject
{
    public new string name;
    public Sprite thumbnail;
    public Placeable placeablePrefab;
}
