using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������� ��� �������� �������� ������������ � ��� ���������� ����.
/// </summary>
public class ParallelAction : EventActionComponent
{
    public override IEnumerator Execute(EventContext ctx)
    {
        var children = GetChildActions();
        if (children.Count == 0) yield break;

        var completed = new bool[children.Count];
        var coroutines = new Coroutine[children.Count];

        for (int i = 0; i < children.Count; i++)
        {
            int index = i;
            var act = children[i];
            if (act == null) { completed[index] = true; continue; }

            coroutines[index] = ctx.manager.StartCoroutine(RunAction(act, ctx, () => completed[index] = true));
        }

        // �����, ���� ��� ����������
        while (!AllCompleted(completed))
        {
            if (ctx.cancelRequested)
            {
                // ��� ������ � ����� OnStop ��� ���� ��� �� ������������� �������� (���� ��� ����������)
                for (int i = 0; i < children.Count; i++)
                {
                    if (!completed[i] && children[i] != null && children[i].CanBeInterrupted)
                    {
                        try { children[i].OnStop(ctx); } catch { }
                    }
                }

                // ��������� ���������� ��������, ����� �������������� ��������� RunAction (finally ���������)
                for (int i = 0; i < coroutines.Length; i++)
                {
                    if (coroutines[i] != null)
                    {
                        try { ctx.manager.StopCoroutine(coroutines[i]); }
                        catch { }
                        coroutines[i] = null;
                    }
                }

                yield break;
            }
            yield return null;
        }
    }

    protected IEnumerator RunAction(EventActionComponent act, EventContext ctx, System.Action onComplete)
    {
        // ��������� IEnumerator �������� � ��� ��� ����������.
        // ���� Execute ����� null � ������� �������� �����������.
        IEnumerator enumerator = null;
        try
        {
            enumerator = act.Execute(ctx);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            onComplete?.Invoke();
            yield break;
        }

        if (enumerator == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        // ���������� try/finally � yield ������ try/finally ��������, � finally �������������� ����������.
        try
        {
            while (true)
            {
                // �������� ����� �������� MoveNext: ���� �������� � ������, finally ������� onComplete
                if (ctx.cancelRequested) yield break;

                bool hasNext;
                object current;
                // ����� MoveNext ����� ��������� ���������� � ����� ��� ������ �������� (Unity ��� ��������).
                // �� ����������� MoveNext � catch � yield ������ (����� ���������� ��������).
                hasNext = enumerator.MoveNext();
                if (!hasNext) break;
                current = enumerator.Current;
                // ����� ���������� �� ���� � ������� yield-���������
                yield return current;
            }
        }
        finally
        {
            // �������������� �������� ��� �����������
            try { onComplete?.Invoke(); } catch { }
        }
    }

    protected List<EventActionComponent> GetChildActions()
    {
        var list = new List<EventActionComponent>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var actions = child.GetComponents<EventActionComponent>();
            foreach (var a in actions)
                list.Add(a);
        }
        return list;
    }

    protected bool AllCompleted(bool[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            if (!arr[i]) return false;
        return true;
    }
}