using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
    [SerializeField] private ShootableSO shootableSO; // Defines the stats of the shootable (health, speed, etc.)
    [SerializeField] private PlaceableSO placeableSO; // This stores the placeable object that will be added to the inventory if it dies

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;
    private Canvas alertCanvas;

    public bool IsDead => isDead;
    public ShootableSO ShootableSO => shootableSO;
    public PlaceableSO PlaceableSO => placeableSO;

    private void Awake()
    {
        currentHealth = shootableSO.maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();
    }

    private void Die()
    {
        isDead = true;
        alertCanvas.enabled = false;
        meshRenderer.material.color = Color.blue; // Here we just change the material to the dead material for testing purposes, this can be changed to whatever logic to handle death
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }
}
