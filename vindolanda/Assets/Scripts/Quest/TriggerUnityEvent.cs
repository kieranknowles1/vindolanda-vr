using System;
using UnityEngine.Events;

namespace Vindolanda.Quest
{
    [Serializable]
    public class TriggerUnityEvent : QuestEvent
    {
        public UnityEvent onTrigger;

        public override void Execute()
        {
            onTrigger?.Invoke();
        }
    }
}