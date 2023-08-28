using UnityEngine;

public abstract class Shootable : MonoBehaviour, IInteractable
{
    [SerializeField] private FurnitureSO furnitureSO;

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;
    private int price;
    private int materialIndex;
    private float scaleFactor;
    private FurnitureAlert furnitureAlert;

    public bool IsDead => isDead;
    public FurnitureSO FurnitureSO => furnitureSO;
    public string InteractActionText => "Pickup";
    public bool Interactable => isDead;

    private void Awake()
    {
        currentHealth = furnitureSO.maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        furnitureAlert = GetComponentInChildren<FurnitureAlert>();

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
        furnitureAlert.OnDead();
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

    public SaveDataFurniture GetInventoryItem()
    {
        return new SaveDataFurniture(furnitureSO.id, scaleFactor, materialIndex, price);
    }

    public void Interact()
    {
        if (isDead)
        {
            HuntingManager.Instance.HuntingInventory.AddItem(GetInventoryItem());
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public struct Hitbox
    {
        public Collider collider;
        public float multiplier;
    }
}
