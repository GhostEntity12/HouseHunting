using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
	public string InteractActionText => (HouseManager.Instance ? "Exit" : "Enter") + " House";
	public bool Interactable => true;

    public void Interact()
	{
		if (HuntingManager.Instance != null)
			HuntingManager.Instance.RespawnInHouse();
		else
			SceneManager.LoadScene(2);
    }
}