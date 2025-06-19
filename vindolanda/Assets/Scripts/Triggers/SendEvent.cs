using Unity.Behavior;
using UnityEngine;

public class SendEvent : TriggerBase
{
    [SerializeReference] public DefaultEvent target;

    protected override void Execute(PlayerController player)
    {
        target.SendEventMessage();
    }
}
