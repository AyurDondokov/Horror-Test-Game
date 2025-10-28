using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] EventSequence activeSequence;
    [SerializeField] EventActionComponent activeAction;
    [SerializeField] bool isPlaying = false;
    
    [Header("Start")]
    [SerializeField] bool PlayOnStart = false;
    [SerializeField] EventSequence startSequence;

    Coroutine runningCoroutine;
    EventContext currentContext;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (PlayOnStart && startSequence != null) PlayEvent(startSequence);
    }
    public void PlayEvent(EventSequence eventSequence)
    {
        PlayEvent(eventSequence, null);
    }
    /// <summary>
    /// Запустить событие. Если уже играется — оно будет остановлено.
    /// initiator — объект, считающийся инициатором (например Player); он будет задан в контексте.
    /// </summary>
    public void PlayEvent(EventSequence seq, GameObject initiator = null)
    {
        if (seq == null) { Debug.LogWarning("PlayEvent called with null sequence"); return; }

        if (isPlaying)
        {
            StopActiveEvent();
        }

        runningCoroutine = StartCoroutine(RunSequence(seq, initiator));
    }

    IEnumerator RunSequence(EventSequence seq, GameObject initiator)
    {
        activeSequence = seq;
        isPlaying = true;
        
        if (seq.lockPlayerDuring && initiator != null)
        {
            var m = initiator.GetComponent<PlayerController>();
            if (m != null) m.LockMovement(true);
        }

        currentContext = new EventContext
        {
            initiator = initiator ?? gameObject,
            manager = this,
            blackboard = new EventBlackboard(),
            mainCamera = Camera.main,
            cancelRequested = false
        };

        var actions = seq.GetActionsInOrder();

        foreach (var action in actions)
        {
            activeAction = action;

            if (currentContext.cancelRequested) break;
            if (action == null) continue;

            IEnumerator it = null;
            try { it = action.Execute(currentContext); } catch (System.Exception ex) { Debug.LogException(ex); continue; }
            if (it == null) continue;

            bool finished = false;
            Coroutine runner = StartCoroutine(RunAndCatch(it, () => finished = true));

            while (!finished)
            {
                if (currentContext.cancelRequested && action.CanBeInterrupted)
                {
                    try { action.OnStop(currentContext); } catch { }
                    if (runner != null) StopCoroutine(runner);
                    break;
                }
                yield return null;
            }

            yield return null;
        }

        if (seq.lockPlayerDuring && initiator != null)
        {
            var m = initiator.GetComponent<PlayerController>();
            if (m != null) m.LockMovement(false);
        }

        activeAction = null;
        activeSequence = null;
        currentContext = null;
        isPlaying = false;
        runningCoroutine = null;
    }

    IEnumerator RunAndCatch(IEnumerator enumerator, System.Action onComplete)
    {
        while (true)
        {
            object cur;
            try
            {
                if (!enumerator.MoveNext()) break;
                cur = enumerator.Current;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                break;
            }
            yield return cur;
        }
        onComplete?.Invoke();
    }

    /// <summary>
    /// Мягко попросить текущий ивент завершиться (с флагом cancelRequested). Action сам обязан корректно отреагировать.
    /// </summary>
    public void StopActiveEvent()
    {
        if (!isPlaying || currentContext == null) return;
        currentContext.cancelRequested = true;

        if (activeAction != null && activeAction.CanBeInterrupted)
        {
            try { activeAction.OnStop(currentContext); } catch { }
        }
    }

    /// <summary>
    /// Полная принудительная остановка: останавливаем корутину менеджера и очищаем состояние.
    /// </summary>
    public void ForceStopActiveEvent()
    {
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);

        if (activeAction != null && currentContext != null)
        {
            try { activeAction.OnStop(currentContext); } catch { }
        }
        activeAction = null;
        activeSequence = null;
        currentContext = null;
        runningCoroutine = null;
        isPlaying = false;
    }

#if UNITY_EDITOR
    public EventSequence ActiveSequence => activeSequence;
    public EventActionComponent ActiveAction => activeAction;
    public bool IsPlaying => isPlaying;
#endif
}
