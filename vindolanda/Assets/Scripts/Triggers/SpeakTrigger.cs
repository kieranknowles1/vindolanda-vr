using UnityEngine;
using Vindolanda.Quest;

public class SpeakTrigger : TriggerBase
{
    public Dialogue dialogue;
    public Speaker speaker;

    protected override void Execute(PlayerController player)
    {
        speaker.Say(dialogue);
    }
}
