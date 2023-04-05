using UnityEngine;

public class HouseManager : MonoBehaviour
{
    private static HouseManager instance;

    public static HouseManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
