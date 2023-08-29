using UnityEngine;

public enum AIType { Prey, Projectile, Charge, Grapple, Slam };
public enum Ability { None, Steal, Alert, Reflect };
public enum AlertRate { Low, Medium, High, Instant };

[CreateAssetMenu(fileName = "Furniture")]

public class FurnitureSO : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public new string name;
    public int maxHealth;
    public int damage;
    public float speed;

	[Header("System")]
	public Sprite thumbnail;
	public Placeable placeablePrefab;
	public Shootable shootablePrefab;

	[Header("Alertness/AI")]
    public ViewConeSO[] senses;
    [Range(0, 100)]
    public float alertnessThreshold1;
    [Range(0, 100)]
    public float alertnessThreshold2;
    [Range(0, 100)]
    public float alertnessThreshold3;
    public float timeBeforeDecay;
    public float alertnessDecayRate;

    [Header("Other")]
    public float attackInterval;
    public bool xray; //can the furniture see the player through obstructions?
    public AIType behavior;
    public AlertRate alertRate;
    public Ability special;
    public Material[] materials;
    public int basePrice;
}
