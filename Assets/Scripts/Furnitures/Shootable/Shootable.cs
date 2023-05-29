using UnityEngine;

public abstract class Shootable : MonoBehaviour, IInteractable
{
    [SerializeField] private FurnitureSO furnitureSO;

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;
    private Canvas alertCanvas;
    private int price;
    private int materialIndex;
    private float scaleFactor;

    public bool IsDead => isDead;
    public FurnitureSO FurnitureSO => furnitureSO;

    private void Awake()
    {
        currentHealth = furnitureSO.maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();

        price = Mathf.RoundToInt(furnitureSO.basePrice * Random.Range(0.5f, 1.5f));
        materialIndex = Random.Range(0, furnitureSO.materials.Length);
        scaleFactor = Random.Range(0.95f, 1.05f);

        meshRenderer.material = furnitureSO.materials[materialIndex];
    }

    private void Start()
    {
        transform.localScale *= scaleFactor;
    }

    public void Die()
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

    public int[] GetHealth()
    {
        int[] healthStatus = { currentHealth, furnitureSO.maxHealth };
        return healthStatus;
    }

    public FurnitureItem GetInventoryItem()
    {
        return new FurnitureItem(furnitureSO.id, scaleFactor, materialIndex, price);
    }

    public void Interact()
    {
        if (isDead)
        {
            HuntingManager.Instance.HuntingInventory.AddItem(GetInventoryItem());
            Destroy(gameObject);
        }
    }
}
