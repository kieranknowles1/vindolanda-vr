using UnityEngine;
using Vindolanda.Quest;

public class SetTourDialogue : TriggerBase
{
    public DialogueEvent setTourDialogue;
    public Dialogue dialogue;

    protected override void Execute(PlayerController player)
    {
        setTourDialogue.SendEventMessage(dialogue);
    }

    protected override void ExecuteExit(PlayerController player)
    {
        setTourDialogue.SendEventMessage(null);
    }
}
