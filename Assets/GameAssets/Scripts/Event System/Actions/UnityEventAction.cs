using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventAction : EventActionComponent
{
    public UnityEvent onInvoke;

    public override IEnumerator Execute(EventContext ctx)
    {
        onInvoke?.Invoke();
        yield break;
    }
}
