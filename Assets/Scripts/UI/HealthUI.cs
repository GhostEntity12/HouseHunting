using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthIconPrefab;

    private void Start()
    {
        for (int i = 0; i < HuntingManager.Instance.MaxHealth; i++)
        {
            Instantiate(healthIconPrefab, transform);
        }
    }

    public void DecrementHealth()
    {
        Transform health = transform.GetChild(0);
        Destroy(health.gameObject);
    }
}
