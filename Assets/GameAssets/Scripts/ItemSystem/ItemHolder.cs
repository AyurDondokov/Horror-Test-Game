using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Позиция предмета в руках")]
    public Vector3 holdLocalPosition = new Vector3(0f, -0.15f, 0.6f);
    public Vector3 holdLocalEuler = Vector3.zero; // Только положительные значения

    private void Reset()
    {
        transform.localPosition = holdLocalPosition;
        transform.localEulerAngles = holdLocalEuler;
    }
}
