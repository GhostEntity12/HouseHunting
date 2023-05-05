using UnityEngine;
using System.Collections.Generic;


public abstract class Shootable : MonoBehaviour
{
    [SerializeField] private FurnitureSO furnitureSO;

    private MeshRenderer meshRenderer;
    private int currentHealth;
    private bool isDead = false;
    private Canvas alertCanvas;
    private float price;
    private int materialIndex;
    private float scaleFactor;

    [System.Serializable]
    public struct Hitbox
    {
        public Collider collider;
        public float multiplier;
    }

    public Hitbox[] Hitboxes;
    public bool IsDead => isDead;
    public FurnitureSO FurnitureSO => furnitureSO;

    private void Awake()
    {
        currentHealth = furnitureSO.maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        alertCanvas = GetComponentInChildren<Canvas>();

        price = furnitureSO.basePrice * Random.Range(0.5f, 1.5f);
        materialIndex = Random.Range(0, meshRenderer.materials.Length);
        scaleFactor = Random.Range(0.95f, 1.05f);

        meshRenderer.material = furnitureSO.materials[materialIndex];
    }

    private void Die()
    {
        isDead = true;
        alertCanvas.enabled = false;
        meshRenderer.material.color = Color.blue; // Here we just change the material to the dead material for testing purposes, this can be changed to whatever logic to handle death
    }

    public void TakeDamage(int damage, Collider hitbox)
    {
        if (isDead) return;
        float damage_mult = 1f;
        foreach (Hitbox hit in Hitboxes)
        {
            if (hit.collider == hitbox)
            {
                damage_mult = hit.multiplier;
            }
        }
        int final_damage = (int)(damage * damage_mult);
        currentHealth -= final_damage;
        print(damage_mult);
        if (currentHealth <= 0) Die();
    }

    public InventoryItem GetInventoryItem()
    {
        return new InventoryItem(furnitureSO.id, scaleFactor, materialIndex, price);
    }
}
