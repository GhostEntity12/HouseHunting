using UnityEngine;

public enum AIType { Prey, Projectile, Charge, Grapple, Slam };
public enum Ability { None, Steal, Alert, Reflect };
public enum AlertRate { Low, Medium, High, Instant };

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
    public AIType behavior;
    public float alertnessThreshold1;
    public float alertnessThreshold2;
    public float alertnessThreshold3;
    public AlertRate alertRate;
    public Ability special;
    public ViewConeSO[] senses;
    public Material[] materials;
    public int basePrice;
    public Sprite thumbnail;
    public Placeable placeablePrefab;
    public Shootable shootablePrefab;
}
