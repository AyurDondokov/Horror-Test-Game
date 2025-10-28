using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class  EventActionComponent : MonoBehaviour
{
    [Tooltip("������������ ��� ���� (��� �������� �������)")]
    public string displayName = "";

    /// <summary>
    /// ��������� ��������; ������� IEnumerator ������� ����������, ����� �������� ����������.
    /// </summary>
    public abstract IEnumerator Execute(EventContext ctx);

    /// <summary>
    /// ����������, ����� ������������������ �����������. ����������� ��� ������� ���������.
    /// </summary>
    public virtual void OnStop(EventContext ctx) { }

    /// <summary>
    /// ����� �� �������� ��� �������� (���� false - �������� ����� ����� ���� �������� ���� ����������)
    /// </summary>
    public virtual bool CanBeInterrupted => true;

    string GetFriendlyName() => !string.IsNullOrEmpty(displayName) ? displayName : name;

    public override string ToString() => $"{GetType().Name} ({GetFriendlyName()})";
}
