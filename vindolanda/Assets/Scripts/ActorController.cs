using System.Text.RegularExpressions;
using Unity.Behavior;
using UnityEngine;
public class ActorSaveData : SaveData
{
    public static readonly RuntimeSerializationUtility.JsonBehaviorSerializer serializer = new();
    // HACK: Agent.Serialize is hardcoded to use indentation. Applying this pattern removes it which
    // reduces output size by 75%
    static readonly Regex cleanupPattern = new(@"\n *");

    public string graphData;

    public ActorSaveData() { }
    public ActorSaveData(ActorController obj) : base(obj)
    {
        graphData = cleanupPattern.Replace(obj.Agent.Serialize(serializer, GuidManager.Instance), "");
    }
}

public class ActorController : Saveable
{
    public BehaviorGraphAgent Agent { get; private set; }
    public Vector3 OriginalPosition { get; private set; }

    protected override void Start()
    {
        base.Start();
        Agent = GetComponent<BehaviorGraphAgent>();
        OriginalPosition = transform.position;
    }

    #region Save Load
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

    #endregion
}
