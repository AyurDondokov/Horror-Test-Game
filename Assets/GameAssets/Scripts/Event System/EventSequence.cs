using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EventSequence : MonoBehaviour
{
    public bool lockPlayerDuring = true;

    public string sequenceId;

    public List<EventActionComponent> GetActionsInOrder()
    {
        var list = new List<EventActionComponent>();

        var selfActions = GetComponents<EventActionComponent>();
        foreach (var a in selfActions) list.Add(a);

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var actions = child.GetComponents<EventActionComponent>();
            foreach (var a in actions) list.Add(a);
        }
        return list;
    }

}
