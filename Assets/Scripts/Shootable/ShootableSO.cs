using UnityEngine;

[CreateAssetMenu(fileName = "New Shootable", menuName = "Shootables")]
public class ShootableSO : ScriptableObject
{
    public new string name;
    public int maxHealth;
    public float speed;
}
