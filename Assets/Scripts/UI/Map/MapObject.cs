using UnityEngine;

public class MapObject : MonoBehaviour, IInteractable
{
    public bool Interactable => true;
    public string InteractActionText => "Open Map";

    public void Interact()
    {
        Map.Instance.OpenMap();
    }
}
