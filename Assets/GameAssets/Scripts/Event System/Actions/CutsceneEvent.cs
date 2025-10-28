using UnityEngine;
using System.Collections;

public class CutsceneEvent : EventActionComponent
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private DialogueUI dialogueUI;

    public override IEnumerator Execute(EventContext ctx)
    { 
        dialogueUI.PlayDialogue(dialogue);
        while (dialogueUI.IsPlaying()) 
        {
            yield return null;
        }
    }
}
