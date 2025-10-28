using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}
