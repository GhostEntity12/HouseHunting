using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Shootable : MonoBehaviour, IInteractable
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
    }

    private void Start()
    {
        transform.localScale *= scaleFactor;
    }

    public void Die()
    {
        isDead = true;
        furnitureAlert.OnDead();

        // disable AI when dead
        WanderAI wanderAI = transform.GetComponent<WanderAI>();
        wanderAI.enabled = false;
        NavMeshAgent navMeshAgent = transform.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;

        // add a rigid body component and apply force to make it topple
        Rigidbody rb = transform.AddComponent<Rigidbody>();
        rb.AddForce(GameManager.Instance.Player.transform.forward * 5, ForceMode.Impulse);
        rb.AddTorque(GameManager.Instance.Player.transform.forward * 5, ForceMode.Impulse);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }


	public SaveDataFurniture GetInventoryItem()
    {
        return new SaveDataFurniture(furnitureSO.id);
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
