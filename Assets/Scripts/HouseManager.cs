using UnityEngine;

public class HouseManager : MonoBehaviour
{
    private static HouseManager instance;

    public static HouseManager Instance { get; private set; }

    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    private void Start() 
    {
    }
}
