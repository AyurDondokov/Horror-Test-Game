using UnityEngine;

[CreateAssetMenu(menuName = "NPC/NPC Data", fileName = "NPCData")]
public class NPCData : ScriptableObject
{
    [Header("Navigation")]
    public float walkSpeed = 3.0f;
    public float acceleration = 8f;
    public float angularSpeed = 120f;
    public float stoppingDistance = 0.8f;
    public float autoRepathInterval = 1.0f;
    public int maxRepathAttempts = 3;

    [Header("Arrival")]
    public float arriveThreshold = 0.4f;
    public float stuckTimeout = 2.0f;
    public float stuckPositionTolerance = 0.1f;

    [Header("Order / Interaction")]
    public string npcId = "npc_default";
    public bool canReceiveItems = true;

    [Header("Debug")]
    public bool debugDraw = false;
}
