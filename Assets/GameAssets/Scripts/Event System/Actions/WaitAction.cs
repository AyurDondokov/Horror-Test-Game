using System.Collections;
using UnityEngine;

public class WaitAction : EventActionComponent
{
    public float seconds = 1f;

    public override IEnumerator Execute(EventContext ctx)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (ctx.cancelRequested) yield break;
            t += Time.deltaTime;
            yield return null;
        }
    }

    public override string ToString() => $"Wait({seconds}s)";
}
