using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Object sceneToLoad;

    public void Interact()
	{
		Debug.Log("InteractedWithDoor");
		if (HuntingManager.Instance != null)
			HuntingManager.Instance.RespawnInHouse();
		else
			SceneManager.LoadScene(2);
    }
}
