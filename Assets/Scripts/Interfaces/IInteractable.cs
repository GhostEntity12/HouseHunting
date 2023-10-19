public interface IInteractable
{
    public bool Interactable { get; }
    public string InteractActionText { get; }

    public void Interact();
}
