using UnityEngine;

public class EventContext
{
    public GameObject initiator;
    public EventManager manager;
    public EventSequence sequence;
    public EventBlackboard blackboard;
    public Camera mainCamera;
    public bool cancelRequested = false;

}
