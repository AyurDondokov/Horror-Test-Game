using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public ItemHolder itemHolder;
    public Transform holdTransform => itemHolder.transform;

    [Header("Raycast / distances")]
    public float interactDistance = 2.5f;
    public LayerMask interactLayerMask;
    public LayerMask placementLayerMask;

    [Header("Placement")]
    [Tooltip("Слой, который будет назначен удерживаемому предмету, чтобы он не коллайдился с игроком")]
    public int heldItemLayer = 7;
    public float placementMaxDistance = 2.0f;

    [Header("Throw charge")]
    public float maxHoldTime = 1.5f;
    public float minThrowForceMultiplier = 0.2f;

    private Item heldItem = null;
    public bool isHoldItem() => heldItem != null;
    public string GetHoldItemId() => heldItem.itemId;

    private float holdStartTime = 0f;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (itemHolder == null) Debug.LogError("ItemHolder not assigned in ItemInteraction");
    }

    private void Update()
    {
        HandleInteractInput();
        HandlePlaceOrThrowInput();
        UpdateHeldItemPosition();
    }

    private IInteractable currentFocused;

    private void HandleInteractInput()
    {
        Ray r = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(r, out RaycastHit hit, interactDistance, interactLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != currentFocused)
            {
                if (currentFocused != null)
                    currentFocused.OnDefocus();

                currentFocused = interactable;

                if (currentFocused != null && currentFocused.IsInteractable())
                    currentFocused.OnFocus();
            }

            if (InputManager.Instance.GetInteractDown())
            {
                Item it = hit.collider.GetComponentInParent<Item>();
                if (it != null)
                {
                    if (isHoldItem()) return;
                    PickUpItem(it);
                    return;
                }

                if (interactable != null && interactable.IsInteractable())
                {
                    interactable.Interact(gameObject);
                    return;
                }
            }
        }
        else
        {
            if (currentFocused != null)
            {
                currentFocused.OnDefocus();
                currentFocused = null;
            }
        }
    }


    private void HandlePlaceOrThrowInput()
    {
        if (heldItem == null) return;

        if (InputManager.Instance.GetPlaceOrThrowDown())
        {
            holdStartTime = Time.time;
        }

        if (InputManager.Instance.GetPlaceOrThrowUp())
        {
            float held = Time.time - holdStartTime;

            float shortPressThreshold = 0.15f;

            if (held <= shortPressThreshold)
            {
                TryPlaceHeldItem();
            }
            else
            {
                ThrowHeldItem(held);
            }
        }
    }

    private void UpdateHeldItemPosition()
    {
        if (heldItem == null) return;

        Transform t = heldItem.transform;
        t.localPosition = Vector3.Lerp(t.localPosition, itemHolder.holdLocalPosition, Time.deltaTime * 15f);
        t.localEulerAngles = Vector3.Lerp(t.localEulerAngles, itemHolder.holdLocalEuler, Time.deltaTime * 15f);
    }

    public void PickUpItem(Item item)
    {
        if (item.isHeld) return;
        heldItem = item;

        Vector3 localPos = itemHolder.holdLocalPosition;
        Quaternion localRot = Quaternion.Euler(itemHolder.holdLocalEuler);

        item.OnPickup(itemHolder.transform, localPos, localRot, heldItemLayer);
    }

    private void TryPlaceHeldItem()
    {
        Ray r = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(r, out RaycastHit hit, placementMaxDistance, placementLayerMask))
        {
            Vector3 halfExtents = heldItem.placementCheckSize * 0.5f;

            Vector3 center = hit.point + hit.normal * (halfExtents.y + heldItem.placementDepthOffset);
            Quaternion boxRot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            bool overlap = Physics.CheckBox(center, halfExtents, boxRot, ~0, QueryTriggerInteraction.Ignore);
            if (!overlap)
            {
                Vector3 placePos = hit.point + hit.normal * (halfExtents.y + heldItem.placementDepthOffset);
                Quaternion placeRot = Quaternion.FromToRotation(heldItem.transform.up, hit.normal) * heldItem.transform.rotation;

                heldItem.OnDrop(placePos, placeRot);
                heldItem = null;
                return;
            }
            else
            {
                // Обратная связь \ звук
                Debug.Log("Не удалось поставить: место занято");
            }
        }
        else
        {
            // Добавить звук
            Debug.Log("Нет подходящей поверхности для постановки");
        }
    }

    private void ThrowHeldItem(float heldDuration)
    {
        float t = Mathf.Clamp01(heldDuration / maxHoldTime);
        float forceMag = Mathf.Lerp(heldItem.minThrowForce, heldItem.maxThrowForce, t);

        Vector3 force = playerCamera.transform.forward * forceMag;
        Vector3 torque = Random.onUnitSphere * heldItem.throwTorque * (0.5f + t);

        heldItem.OnThrow(force, torque);
        heldItem = null;
    }

    public void DestroyHeldItem()
    {
        Destroy(heldItem.gameObject);
        heldItem = null;
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null || heldItem == null) return;

        Ray r = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(r, out RaycastHit hit, placementMaxDistance, placementLayerMask))
        {
            Vector3 halfExtents = heldItem.placementCheckSize * 0.5f;
            Vector3 center = hit.point + hit.normal * (halfExtents.y + heldItem.placementDepthOffset);
            Quaternion boxRot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Gizmos.color = Color.yellow;
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, boxRot, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
            Gizmos.matrix = old;
        }
    }
}
