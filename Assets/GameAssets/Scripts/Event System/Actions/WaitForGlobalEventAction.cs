using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ждёт глобального события eventName. При получении payload (object) — по желанию кладёт его в ctx.blackboard под заданным ключом.
/// </summary>
public class WaitForGlobalEventAction : EventActionComponent
{
    [Tooltip("Имя глобального события для ожидания")]
    public string eventName;

    [Tooltip("Если >0 — прекратить ожидание через заданное время.")]
    public float timeout = 0f;

    [Tooltip("Если true — при получении payload запишет его в blackboard под этим ключом")]
    public bool capturePayloadToBlackboard = false;

    [Tooltip("Ключ для blackboard (если capturePayloadToBlackboard == true)")]
    public string blackboardKey = "eventPayload";

    bool received = false;
    object receivedPayload = null;

    Action<object> handler;

    public override IEnumerator Execute(EventContext ctx)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning($"{name}: eventName не задан");
            yield break;
        }

        received = false;
        receivedPayload = null;

        handler = (payload) =>
        {
            receivedPayload = payload;
            received = true;
        };

        GlobalEventBus.Subscribe(eventName, handler);

        float timer = 0f;
        while (!received)
        {
            if (ctx != null && ctx.cancelRequested) break;
            if (timeout > 0f)
            {
                timer += Time.deltaTime;
                if (timer >= timeout) break;
            }
            yield return null;
        }

        GlobalEventBus.Unsubscribe(eventName, handler);

        if (received && capturePayloadToBlackboard && ctx != null && ctx.blackboard != null)
        {
            ctx.blackboard.Set(blackboardKey, receivedPayload);
        }
    }

    public override void OnStop(EventContext ctx)
    {
        // ensure unsubscription
        if (handler != null) GlobalEventBus.Unsubscribe(eventName, handler);
        received = true;
    }
}
