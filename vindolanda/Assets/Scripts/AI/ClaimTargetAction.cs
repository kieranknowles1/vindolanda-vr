using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ClaimTarget", story: "Claim a [Target] from [Manager]", category: "Action/Find", id: "909a0bac3a62b2f813745178284cb6ed")]
public partial class ClaimTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<ClaimManager> Manager;

    protected override Status OnStart()
    {
        Target.Value = Manager.Value.Claim();

        return Target.Value != null ? Status.Success : Status.Failure;
    }
}

