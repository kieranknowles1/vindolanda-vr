using UnityEngine;
using Vindolanda.Quest;

[Tooltip("Trigger an event when the trigger is entered.")]
public class EventTrigger : TriggerBase
{
    public QuestEvent onEnter;

    protected override void Execute(PlayerController player)
    {
        onEnter.Execute();
    }
}
