using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Запускает все дочерние действия одновременно и ждёт завершения всех.
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

        // ждать, пока все завершатся
        while (!AllCompleted(completed))
        {
            if (ctx.cancelRequested)
            {
                // при отмене — вызов OnStop для всех ещё не завершившихся действий (если они прерываемы)
                for (int i = 0; i < children.Count; i++)
                {
                    if (!completed[i] && children[i] != null && children[i].CanBeInterrupted)
                    {
                        try { children[i].OnStop(ctx); } catch { }
                    }
                }

                // остановим запущенные корутины, чтобы гарантированно завершить RunAction (finally сработает)
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
        // Запускаем IEnumerator действия и ждём его выполнения.
        // Если Execute вернёт null — считаем действие завершённым.
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

        // Используем try/finally — yield внутри try/finally разрешён, и finally гарантированно выполнится.
        try
        {
            while (true)
            {
                // Проверка перед попыткой MoveNext: если отменено — выйдем, finally вызовет onComplete
                if (ctx.cancelRequested) yield break;

                bool hasNext;
                object current;
                // Вызов MoveNext может выбросить исключение — пусть оно прервёт корутину (Unity его логирует).
                // Не оборачиваем MoveNext в catch с yield внутри (иначе компилятор ругается).
                hasNext = enumerator.MoveNext();
                if (!hasNext) break;
                current = enumerator.Current;
                // Отдаём управление на кадр с текущим yield-значением
                yield return current;
            }
        }
        finally
        {
            // Гарантированно помечаем как завершённое
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