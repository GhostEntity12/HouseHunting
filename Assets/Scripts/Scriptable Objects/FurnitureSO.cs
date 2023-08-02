using UnityEngine;

[CreateAssetMenu(fileName = "Furniture")]
public class FurnitureSO : ScriptableObject
{
    public string id;
    public new string name;
    public int maxHealth;
    public int damage;
    public float speed;
    public float attackInterval;
    public bool xray; //can the furniture see the player through obstructions?
    public SenseSO[] senses;
    public Material[] materials;
    public int basePrice;
    public Sprite thumbnail;
    public Placeable placeablePrefab;
    public Shootable shootablePrefab;
}
