using UnityEngine;
using UnityEngine.Events;


[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("General")]
    [Tooltip("Optional local point used as 'interaction point' (leave empty to use transform.position)")]
    public Transform interactionPoint;


    [Tooltip("If true, this interactable temporarily cannot be used.")]
    public bool locked = false;


    [Header("Editor hooks")]
    public UnityEvent onFocus;
    public UnityEvent onDefocus;
    public UnityEvent onInteract;

    public virtual void OnFocus()
    {
        onFocus?.Invoke();
    }

    public virtual void OnDefocus()
    {
        onDefocus?.Invoke();
    }

    public abstract void Interact(GameObject interactor);

    public virtual Vector3 GetInteractionPoint()
    {
        return interactionPoint != null ? interactionPoint.position : transform.position;
    }

    public virtual bool IsInteractable()
    {
        return gameObject.activeInHierarchy && !locked;
    }
}