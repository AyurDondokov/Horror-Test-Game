using UnityEngine;

public interface IInteractable
{
    void OnFocus();

    void OnDefocus();

    void Interact(GameObject interactor);

    Vector3 GetInteractionPoint();

    bool IsInteractable();
}