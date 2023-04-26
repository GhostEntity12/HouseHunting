using UnityEngine;

[CreateAssetMenu(fileName = "Shootable_Furniture", menuName = "Furniture/Shootable")]
public class ShootableSO : ScriptableObject
{
    public new string name;
    public int maxHealth;
    public int damage;
    public float speed;
    public float attackInterval;
    public float perceptionRadius; //how far the AI can see the player
}
