using UnityEngine;
using Vindolanda.Quest;

[Tooltip("Trigger an event when the trigger is entered.")]
public class EventTrigger : Saveable
{
    public class EventTriggerData : SaveData
    {
        public bool done;
        public EventTriggerData() { }
        public EventTriggerData(EventTrigger t) : base(t) { done = t.done; }
    }

    public QuestEvent onEnter;
    public bool singleUse;

    protected bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (!other.GetComponent<PlayerController>()) return;

        onEnter.Execute();

        if (singleUse) done = true;
    }

    public override SaveData Save()
    {
        return new EventTriggerData(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        done = ((EventTriggerData)data).done;
    }
}
