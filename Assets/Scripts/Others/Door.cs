using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
	{
		if (HuntingManager.Instance != null)
			HuntingManager.Instance.RespawnInHouse();
		else
			SceneManager.LoadScene(2);
    }
}