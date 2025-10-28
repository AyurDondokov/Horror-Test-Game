using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("������� �������� � �����")]
    public Vector3 holdLocalPosition = new Vector3(0f, -0.15f, 0.6f);
    public Vector3 holdLocalEuler = Vector3.zero; // ������ ������������� ��������

    private void Reset()
    {
        transform.localPosition = holdLocalPosition;
        transform.localEulerAngles = holdLocalEuler;
    }
}
