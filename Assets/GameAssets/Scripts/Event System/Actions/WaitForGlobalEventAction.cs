using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ��� ����������� ������� eventName. ��� ��������� payload (object) � �� ������� ����� ��� � ctx.blackboard ��� �������� ������.
/// </summary>
public class WaitForGlobalEventAction : EventActionComponent
{
    [Tooltip("��� ����������� ������� ��� ��������")]
    public string eventName;

    [Tooltip("���� >0 � ���������� �������� ����� �������� �����.")]
    public float timeout = 0f;

    [Tooltip("���� true � ��� ��������� payload ������� ��� � blackboard ��� ���� ������")]
    public bool capturePayloadToBlackboard = false;

    [Tooltip("���� ��� blackboard (���� capturePayloadToBlackboard == true)")]
    public string blackboardKey = "eventPayload";

    bool received = false;
    object receivedPayload = null;

    Action<object> handler;

    public override IEnumerator Execute(EventContext ctx)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning($"{name}: eventName �� �����");
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
