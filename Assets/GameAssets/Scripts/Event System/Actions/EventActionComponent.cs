using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class  EventActionComponent : MonoBehaviour
{
    [Tooltip("ќтображаемое им€ шага (дл€ удобства отладки)")]
    public string displayName = "";

    /// <summary>
    /// ¬ыполнить действие; верните IEnumerator который завершитс€, когда действие закончитс€.
    /// </summary>
    public abstract IEnumerator Execute(EventContext ctx);

    /// <summary>
    /// ¬ызываетс€, когда последовательность прерываетс€. »спользуйте дл€ очистки состо€ний.
    /// </summary>
    public virtual void OnStop(EventContext ctx) { }

    /// <summary>
    /// ћожно ли прервать это действие (если false - менеджер будет ждать пока действие само завершитс€)
    /// </summary>
    public virtual bool CanBeInterrupted => true;

    string GetFriendlyName() => !string.IsNullOrEmpty(displayName) ? displayName : name;

    public override string ToString() => $"{GetType().Name} ({GetFriendlyName()})";
}
