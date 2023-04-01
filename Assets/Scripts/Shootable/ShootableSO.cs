using UnityEngine;

[CreateAssetMenu(fileName = "Shootable_Furniture", menuName = "Furniture/Shootable")]
public class ShootableSO : ScriptableObject
{
    public new string name;
    public int maxHealth;
    public float speed;
}
