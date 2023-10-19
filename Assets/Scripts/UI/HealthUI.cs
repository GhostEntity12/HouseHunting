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

    public void SetHealth(float health)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform ithChild = transform.GetChild(i);
            if (health >= 1)
                ithChild.gameObject.SetActive(true);
            else if (health > 0)
            {
                ithChild.gameObject.SetActive(true);
                ithChild.GetComponent<Image>().fillAmount = health;
            }
            health--;
        }
    }
}
