using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
    [SerializeField] private ShootableSO shootableSO; // Defines the stats of the shootable (health, speed, etc.)
    [SerializeField] private Placeable placeablePrefab; // This stores the placeable object that will be added to the inventory if it dies
    [SerializeField] private Material deadMaterial; // This is the material that will be applied to the mesh renderer when it dies (here just for testing)

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;

    public bool IsDead => isDead;
    public Placeable Placeable => placeablePrefab;

    void Awake()
    {
        currentHealth = shootableSO.maxHealth;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{shootableSO.name} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        meshRenderer.material = deadMaterial;
    }
}
