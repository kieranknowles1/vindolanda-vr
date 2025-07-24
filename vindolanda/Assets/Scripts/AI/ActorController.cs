using System.Text.RegularExpressions;
using Unity.Behavior;
using UnityEngine;
using Vindolanda.Animation;
using Vindolanda.Quest;
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
        if (obj.Agent.Graph == null) {
            graphData = null;
            return;
        }
        graphData = cleanupPattern.Replace(obj.Agent.Serialize(serializer, GuidManager.Instance), "");
    }
}

public class ActorController : Saveable, IHitTarget
{
    [Header("Dialogue")]
    public Dialogue hitDialogue;

    [Header("Boilerplate")]
    public Animator animator;
    public ActorAnimator ActorAnimator { get; private set; }
    public Speaker Speaker { get; private set; }
    public BehaviorGraphAgent Agent { get; private set; }
    public Vector3 OriginalPosition { get; private set; }

    protected void Awake()
    {
        Agent = GetComponent<BehaviorGraphAgent>();
        OriginalPosition = transform.position;
        ActorAnimator = animator.GetComponent<ActorAnimator>();
        Speaker = GetComponent<Speaker>();
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
        if (actorData.graphData != null)
            Agent.Deserialize(actorData.graphData, ActorSaveData.serializer, GuidManager.Instance);
    }

    #endregion

    public void OnHit(IWeapon _weapon)
    {
        Speaker.Say(hitDialogue);
    }
}
