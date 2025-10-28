using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public string itemId = "Taco";
    [Header("Placement check")]
    [Tooltip("Размер бокса используемого в Physics.CheckBox (в локальных координатах предмета)")]
    public Vector3 placementCheckSize = Vector3.one;

    [Tooltip("Небольшое смещение от поверхности, чтобы CheckBox не задевал саму поверхность")]
    public float placementDepthOffset = 0.02f;

    [Header("Бросок")]
    public float minThrowForce = 2f;
    public float maxThrowForce = 15f;
    public float throwTorque = 2f;

    [HideInInspector] public Rigidbody rb;

    [HideInInspector] public bool isHeld = false;
    [HideInInspector] public int originalLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalLayer = gameObject.layer;
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup(Transform parent, Vector3 localPosition, Quaternion localRotation, int heldLayer)
    {
        isHeld = true;
        rb.isKinematic = true;
        transform.SetParent(parent, true);
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
        gameObject.layer = heldLayer;
    }

    public void OnDrop(Vector3 worldPosition, Quaternion worldRotation)
    {
        isHeld = false;
        transform.SetParent(null, true);
        transform.position = worldPosition;
        transform.rotation = worldRotation;
        rb.isKinematic = false;
        gameObject.layer = originalLayer;
    }

    public void OnThrow(Vector3 force, Vector3 torque)
    {
        isHeld = false;
        transform.SetParent(null, true);
        gameObject.layer = originalLayer;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.VelocityChange);
        rb.AddTorque(torque, ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, placementCheckSize);
        Gizmos.matrix = old;
    }
}
