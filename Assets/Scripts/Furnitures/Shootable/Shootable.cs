using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Shootable : MonoBehaviour, IInteractable
{
    [SerializeField] private FurnitureSO furnitureSO;

    private int currentHealth;
    private bool isDead = false;
    private FurnitureAlert furnitureAlert;

    public bool IsDead => isDead;
    public FurnitureSO FurnitureSO => furnitureSO;
    public string InteractActionText => "Pickup";
    public bool Interactable => isDead;

	private void Awake()
    {
        currentHealth = furnitureSO.maxHealth;
        furnitureAlert = GetComponentInChildren<FurnitureAlert>();
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
