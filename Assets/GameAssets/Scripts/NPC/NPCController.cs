using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public NPCData data;

    private NavMeshAgent agent;
    private Vector3 currentTarget;
    private Action onArrived;
    private Action onFailed;
    private Transform LookTarget;
    public enum State { Idle, Moving, Waiting, Interacting, Failed }
    private State state = State.Idle;
    private Transform FollowTarget;
    public State GetState() => state;

    private Vector3 lastCheckedPos;
    private float lastCheckedTime;
    private int repathAttempts = 0;

    public event Action<NPCController> OnReachedTarget;
    public event Action<NPCController> OnFailedToReach;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (data == null) Debug.LogWarning($"NPCData not assigned on {name}");
        ApplyDataToAgent();
    }

    private void ApplyDataToAgent()
    {
        if (agent == null || data == null) return;
        agent.speed = data.walkSpeed;
        agent.acceleration = data.acceleration;
        agent.angularSpeed = data.angularSpeed;
        agent.stoppingDistance = data.stoppingDistance;
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (state == State.Moving)
        {
            if (FollowTarget != null)
                GoTo(FollowTarget);

            HandleMoving();
        }
        else if (state == State.Waiting)
        {
            if (LookTarget != null)
            {
                transform.LookAt(new Vector3(LookTarget.position.x, transform.position.y, LookTarget.position.z));
            }
        }
    }

    private void HandleMoving()
    {
        if (agent.desiredVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.desiredVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, data.angularSpeed * Time.deltaTime);
        }

        if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(data.arriveThreshold, agent.stoppingDistance))
        {
            CompleteArrival();
            return;
        }

        if (Time.time - lastCheckedTime > data.stuckTimeout)
        {
            float dist = Vector3.Distance(transform.position, lastCheckedPos);
            lastCheckedTime = Time.time;
            lastCheckedPos = transform.position;

            if (dist < data.stuckPositionTolerance)
            {
                repathAttempts++;
                if (repathAttempts <= data.maxRepathAttempts)
                {
                    TryRepath();
                }
                else
                {
                    FailToReach();
                }
            }
            else
            {
                repathAttempts = 0;
            }
        }
    }

    private void TryRepath()
    {
        if (data.debugDraw) Debug.Log($"{name}: TryRepath attempt {repathAttempts}");
        NavMeshPath path = new NavMeshPath();
        Vector3 sampleTarget = currentTarget;
        sampleTarget += UnityEngine.Random.insideUnitSphere * 0.5f;
        NavMesh.CalculatePath(transform.position, sampleTarget, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
        else
        {
            agent.SetDestination(currentTarget);
        }
    }

    private void CompleteArrival()
    {
        state = State.Waiting;
        agent.ResetPath();
        onArrived?.Invoke();
        OnReachedTarget?.Invoke(this);

        onArrived = null;
        onFailed = null;
    }

    private void FailToReach()
    {
        state = State.Failed;
        agent.ResetPath();
        onFailed?.Invoke();
        OnFailedToReach?.Invoke(this);
        onArrived = null;
        onFailed = null;
    }

    public void LookAt(Transform target)
    {
        LookTarget = target;
    }
    public void StopLooking()
    {
        LookTarget = null;
    }
    public void Follow(Transform target)
    {
        FollowTarget = target;
        state = State.Moving;
    }
    public void GoTo(Transform target)
    {
        GoTo(target.position);
    }

    /// <summary>
    /// Попросить NPC подойти к точке target. Возвращает true, если удалось установить цель
    /// </summary>
    public bool GoTo(Vector3 target, Action arrivedCallback = null, Action failedCallback = null)
    {
        if (data == null)
        {
            Debug.LogWarning("NPCData is null; cannot GoTo");
            return false;
        }

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target, out hit, 2.0f, NavMesh.AllAreas))
        {
            failedCallback?.Invoke();
            return false;
        }

        currentTarget = hit.position;
        onArrived = arrivedCallback;
        onFailed = failedCallback;
        repathAttempts = 0;
        lastCheckedPos = transform.position;
        lastCheckedTime = Time.time;

        bool ok = agent.SetDestination(currentTarget);
        if (!ok)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, currentTarget, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
                ok = true;
            }
            else
            {
                failedCallback?.Invoke();
                return false;
            }
        }

        state = State.Moving;
        return ok;
    }

    /// <summary>
    /// Прерывает текущее движение.
    /// </summary>
    public void Interrupt()
    {
        agent.ResetPath();
        state = State.Idle;
        onArrived = null;
        onFailed = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (data != null && data.debugDraw && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentTarget, 0.15f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.stoppingDistance);
        }
    }
}
