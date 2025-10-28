using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAction : ParallelAction
{
    public override IEnumerator Execute(EventContext ctx)
    {
        var children = GetChildActions();
        if (children.Count == 0) yield break;

        var completed = new bool[children.Count];
        var coroutines = new Coroutine[children.Count];

        int index = (int)Random.Range(0, children.Count);

        var act = children[index];
        if (act == null) { completed[index] = true; }

        coroutines[index] = ctx.manager.StartCoroutine(RunAction(act, ctx, () => completed[index] = true));

        while (!completed[index])
        {
            yield return null;
        }
    }
}
