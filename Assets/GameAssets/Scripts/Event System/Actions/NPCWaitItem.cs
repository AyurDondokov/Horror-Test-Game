using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NPCWaitItem : EventActionComponent
{
    [SerializeField] private NPCItemReceiver receiver;
    [SerializeField] private string itemId;
    public override IEnumerator Execute(EventContext ctx)
    {
        receiver.StartReceive(itemId);

        while (!receiver.IsReceived())
        {
            yield return null;
        }
    }
}
