using UnityEngine;

public class GiveItemInteractable : InteractableBase
{
    [SerializeField] protected GameObject createItem;
    protected ItemInteraction itemInteraction;

    public override void Interact(GameObject interactor)
    { 
        itemInteraction = interactor.GetComponent<ItemInteraction>();
        if (itemInteraction == null) { Debug.Log("Not Found ItemInteractor on Player"); return; }
        if (itemInteraction.isHoldItem()) { return; }

        CreateItem();
    }

    public void CreateItem()
    {
        if (createItem == null) { Debug.Log("No item for create"); return; }
        if (itemInteraction == null) { Debug.Log("No ItemInteraction for give item"); return; }

        Item item = Instantiate(createItem).GetComponent<Item>();
        if (item == null) { Debug.Log("Created item don't have component Item"); return; }

        itemInteraction.PickUpItem(item);
    }
}
