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
    public float height; //vertical offset of its senses, essentially behaving like a "head". This can be left as 0 if not needed.
    public bool xray; //can the furniture see the player through obstructions?
    public float perceptionRadius; //how far the AI can see the player
    public float sneakingPerceptionRadius; //how far the AI can see the player if they're sneaking
    public int fieldOfView; //at what angle can the AI see the player
    public float gunshotHearingRadius; //how far can the AI hear player gunshots
    public float walkingHearingRadius; //how far can the AI hear player walking
    public float sneakingHearingRadius; //how far can the AI hear player sneaking
    public Material[] materials;
    public int basePrice;
    public Sprite thumbnail;
    public Placeable placeablePrefab;
}
