using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
    [SerializeField] private ShootableSO shootableSO; // Defines the stats of the shootable (health, speed, etc.)
    [SerializeField] private PlaceableSO placeableSO; // This stores the placeable object that will be added to the inventory if it dies
    [SerializeField] private Material deadMaterial; // This is the material that will be applied to the mesh renderer when it dies (here just for testing)

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;

    public bool IsDead => isDead;
    public PlaceableSO PlaceableSO => placeableSO;

    void Awake()
    {
        currentHealth = shootableSO.maxHealth;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        meshRenderer.material = deadMaterial; // Here we just change the material to the dead material for testing purposes, this can be changed to whatever logic to handle death
    }
}
