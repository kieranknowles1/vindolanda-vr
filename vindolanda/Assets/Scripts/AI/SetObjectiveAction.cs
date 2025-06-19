using System;
using Unity.Behavior;
using UnityEngine;
using Vindolanda.Quest;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetObjective", story: "Set [Quest] objective to [Objective]", category: "Quest", id: "34efa78fc96b22a558d5a8d85bcd1fba")]
public partial class SetObjectiveAction : Action
{
    [SerializeReference] public BlackboardVariable<Quest> Quest;
    [SerializeReference] public BlackboardVariable<Objective> Objective;

    protected override Status OnStart()
    {
        GameConstants.Instance.QuestController.GetState(Quest.Value).CurrentObjective = Objective.Value;
        return Status.Success;
    }
}

