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

    public bool PlayerPresent { get; private set; } = false;

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
        PlayerPresent = true;
        Execute(player); 

        // This is set on exit. It should be impossible for a player to re-enter the trigger without first exiting
        // it
        //if (singleUse) done = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (done) return;
        if (!other.TryGetComponent<PlayerController>(out var player)) return;
        PlayerPresent = false;
        ExecuteExit(player);

        if (singleUse) done = true;
    }

    protected abstract void Execute(PlayerController player);
    protected virtual void ExecuteExit(PlayerController player) { /* do nothing */ }
}
