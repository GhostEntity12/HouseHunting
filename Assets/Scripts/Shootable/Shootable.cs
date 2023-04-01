using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField] private ShootableSO shootableSO;
    [SerializeField] private Material deadMaterial;

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;

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
