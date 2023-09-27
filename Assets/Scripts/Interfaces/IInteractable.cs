public interface IInteractable
{
    bool Interactable { get; }
    string InteractActionText { get; }

    void Interact();
}
