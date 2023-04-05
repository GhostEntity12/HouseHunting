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

        // find every placeable object in the scene and enable their colliders
        Placeable[] placeables = FindObjectsOfType<Placeable>();
        foreach (Placeable placeable in placeables) {
            Collider collider = placeable.GetComponent<Collider>();
            if (collider != null) {
                collider.enabled = true;
            }
        }
    }
}
