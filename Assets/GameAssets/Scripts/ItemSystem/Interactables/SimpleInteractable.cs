using UnityEngine;


public class SimpleInteractable : InteractableBase
{
    public bool isInfinity;
    public bool isWasUsed;

    public override void Interact(GameObject interactor)
    {
        if (!isWasUsed)
        {
            onInteract?.Invoke();
            if (!isInfinity)
                isWasUsed = true;
        }
    }
}