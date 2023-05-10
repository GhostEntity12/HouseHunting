using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Object sceneToLoad;

    public void Interact()
    {
        if (sceneToLoad != null)
        {
            // if we are in hunting scene, call respawn in house instead
            if (HuntingManager.Instance != null)
                HuntingManager.Instance.RespawnInHouse();
            else
                SceneManager.LoadScene(sceneToLoad.name);
        }
        else
            Debug.LogError("No scene to load");
    }
}
