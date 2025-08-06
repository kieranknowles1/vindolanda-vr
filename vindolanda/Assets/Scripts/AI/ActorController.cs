using System.Collections;
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

public class ActorController : Saveable, IHitTarget, ISpeechListener
{
    [Header("Dialogue")]
    public Dialogue hitDialogue;

    [Tooltip("If set, actor can be spoken to by sending this event")]
    public DefaultEvent speakEvent;
    [Tooltip("If set, actor will use this graph when speaking")]
    public BehaviorGraph speakGraph;

    public Animator Animator { get; private set; }
    public ActorAnimator ActorAnimator { get; private set; }
    public Speaker Speaker { get; private set; }
    public BehaviorGraphAgent Agent { get; private set; }
    public Vector3 OriginalPosition { get; private set; }

    public bool PlayerCanSpeakTo(PlayerController player) => (speakEvent || speakGraph) && player.transform.GetDistance(transform) < 10.0f;
    public bool ForceSpeak => false;
    public float MaxSpeechDistance => 10.0f;
    [SerializeField] float speechPriority = 1.0f;
    public float SpeechPriority => speechPriority;

    BehaviorGraph defaultGraph;
    BehaviorGraph overrideGraph;
    public BehaviorGraph OverrideGraph
    {
        get => overrideGraph;
        set {
            overrideGraph = value;
            if (value == null)
                Agent.Graph = defaultGraph;
            else
                Agent.Graph = overrideGraph;
        }
    }

    public bool allowGenericQuests;


    protected void Awake()
    {
        if (speakEvent || speakGraph)
            GameConstants.Instance.Player.speechTargets.Add(this);

        Agent = GetComponent<BehaviorGraphAgent>();
        OriginalPosition = transform.position;
        Animator = GetComponentInChildren<Animator>();
        ActorAnimator = Animator.GetComponent<ActorAnimator>();
        Speaker = GetComponent<Speaker>();

        defaultGraph = Agent.Graph;
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

    IEnumerator StopOverrideWhenComplete(BehaviorGraph overrideGraph)
    {
        do
        {
            yield return new WaitForSeconds(1.0f);
        } while (Agent.Graph.IsRunning && overrideGraph == OverrideGraph);
        if (OverrideGraph == overrideGraph)
            OverrideGraph = null;
    }

    public void Speak(PlayerController player)
    {
        if (speakEvent)
            speakEvent.SendEventMessage();
        if (speakGraph)
        {
            OverrideGraph = speakGraph;
            StartCoroutine(StopOverrideWhenComplete(overrideGraph));
        }
    }
}
