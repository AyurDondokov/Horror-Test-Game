using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class ConditionWaitAction : EventActionComponent
{
    public bool isConditionApply;
    [SerializeField] private UnityEvent OnApply;

    public void SetBool(bool value)
    {
        isConditionApply = value;
    }

    public override IEnumerator Execute(EventContext ctx)
    {
        isConditionApply = false;

        while (!isConditionApply)
        {
            OnApply.Invoke();
            yield return null;
        }
    }
}
