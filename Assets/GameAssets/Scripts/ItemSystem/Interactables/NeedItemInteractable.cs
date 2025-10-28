using UnityEngine;

public class NeedItemInteractable : GiveItemInteractable
{
    [SerializeField] protected string needItemId;
    public override void Interact(GameObject interactor)
    {
        itemInteraction = interactor.GetComponent<ItemInteraction>();
        if (itemInteraction == null) { Debug.Log("Not Found ItemInteractor on Player"); return; }
        if (!itemInteraction.isHoldItem()) { return; }
        if (itemInteraction.GetHoldItemId() == needItemId)
        {
            itemInteraction.DestroyHeldItem();
            CreateItem();
        }
    }
}
