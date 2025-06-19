using UnityEngine;

// Base trigger that does something when the player enters
// Optionally single use
public abstract class TriggerBase : Saveable
{
    public class TriggerData : SaveData
    {
        public bool done;
        public TriggerData() { }
        public TriggerData(TriggerBase t) : base(t) { done = t.done; }
    }

    public bool singleUse;

    protected bool done = false;

    public override SaveData Save()
    {
        return new TriggerData(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        done = ((TriggerData)data).done;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (!other.TryGetComponent<PlayerController>(out var player)) return;
        Execute(player); 

        if (singleUse) done = true;
    }

    protected abstract void Execute(PlayerController player);
}
