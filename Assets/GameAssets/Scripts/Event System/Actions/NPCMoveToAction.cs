using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NPCMoveToAction : EventActionComponent
{
    public NPCController controller;
    public Transform target;
    public UnityEvent OnStopMove;
    public override IEnumerator Execute(EventContext ctx)
    {
        controller.GoTo(target);
        while (controller.GetState() != NPCController.State.Waiting) 
        {
            yield return null;
        }
        OnStopMove.Invoke();
    }

}
