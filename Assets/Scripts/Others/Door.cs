using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	public string InteractActionText => (HouseManager.Instance ? "Exit House" : "Enter House");
	public bool Interactable => true;

    public void Interact()
	{
		if (HuntingManager.Instance)
			HuntingManager.Instance.RespawnInHouse();
		else if (!HouseManager.Instance.HoldingPlaceable)
			HouseManager.Instance.LoadHuntingScene();
    }
}