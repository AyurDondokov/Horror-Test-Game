using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class TriggerWait : EventActionComponent
{
    public bool isTriggered;
    [SerializeField] private UnityEvent OnTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            isTriggered = true;
        }
    }
    public override IEnumerator Execute(EventContext ctx)
    {
        while (!isTriggered)
        {
            OnTriggered.Invoke();
            yield return null;
        }
    }
}
