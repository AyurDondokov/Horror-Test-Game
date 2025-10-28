using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class NPCItemReceiver : MonoBehaviour
{
    [Tooltip("ID предмета, который воспринимается как 'заказ' (например 'Taco')")]
    public string acceptedId = "Taco";
    
    public bool isDestroyItem = true;
    public UnityEvent OnReceived;

    public event Action<Item> OnItemReceived;
    public event Action<GameObject> OnOtherObjectReceived;

    private bool isReceived = false;
    public bool IsReceived() => isReceived;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"{name}: NPCItemReceiver - not trigger collider");
        }
    }

    public void StartReceive(string newId)
    {
        acceptedId = newId;
        isReceived = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReceived)
        {
            if (other.transform.IsChildOf(transform)) return;

            Item item = other.GetComponentInParent<Item>();
            if (item != null && item.itemId == acceptedId)
            {
                Received(item);
                return;
            }
        }
    }

    private void Received(Item item)
    {
        isReceived = true;

        OnReceived.Invoke();
        OnItemReceived?.Invoke(item);

        if (isDestroyItem) Destroy(item.gameObject);
    }
}
