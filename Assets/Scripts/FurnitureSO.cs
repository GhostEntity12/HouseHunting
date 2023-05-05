using UnityEngine;

[CreateAssetMenu(fileName = "Shootable_Furniture", menuName = "Furniture")]
public class FurnitureSO : ScriptableObject
{
    public string id;
    public new string name;
    public int maxHealth;
    public int damage;
    public float speed;
    public float attackInterval;
    public float perceptionRadius; //how far the AI can see the player
    public Material[] materials;
    public int basePrice;
    public Sprite thumbnail;
    public Placeable placeablePrefab;
}