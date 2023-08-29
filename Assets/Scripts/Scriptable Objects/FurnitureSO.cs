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
	public float alertnessThreshold1 = 33;
	[Range(0, 100)]
	public float alertnessThreshold2 = 66;
	[Range(0, 100)]
	public float alertnessThreshold3 = 100;
	public float timeBeforeDecay = 5;
	public float alertnessDecayRate = 10;
	public float sightAlertnessRate = 25;

	[Header("Other")]
	public Ability special;
	public Material[] materials;
	public int basePrice;
}
