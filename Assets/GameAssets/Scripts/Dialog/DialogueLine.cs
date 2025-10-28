using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public float typingSpeed = 0.03f;
}
