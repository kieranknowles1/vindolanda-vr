using UnityEngine;
using UnityEngine.Events;

[Tooltip("Trigger an event when the trigger is entered.")]
public class EventTrigger : TriggerBase
{
    public UnityEvent onEnter;

    protected override void Execute(PlayerController player)
    {
        onEnter?.Invoke();
    }
}
