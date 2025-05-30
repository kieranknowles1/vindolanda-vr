using Unity.Behavior;
using Unity.VisualScripting;

public class ActorSaveData : SaveData
{
    public static readonly RuntimeSerializationUtility.JsonBehaviorSerializer serializer = new();

    public string graphData;

    public ActorSaveData() { }
    public ActorSaveData(Saveable obj) : base(obj)
    {
        var actor = (ActorSave)obj;
        graphData = actor.Agent.Serialize(serializer, GuidManager.Instance);
    }
}

public class ActorSave : Saveable
{
    public BehaviorGraphAgent Agent { get; private set; }

    protected override void Start()
    {
        base.Start();
        Agent = GetComponent<BehaviorGraphAgent>();
    }

    public override SaveData Save()
    {
        return new ActorSaveData(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        var actorData = (ActorSaveData)data;
        Agent.Deserialize(actorData.graphData, ActorSaveData.serializer, GuidManager.Instance);
    }
}
