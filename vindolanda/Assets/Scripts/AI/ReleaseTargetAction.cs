using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ReleaseTarget", story: "Release [Target] to [Manager]", category: "Action/Find", id: "0bb45b22407268552e077991d6c7cee8")]
public partial class ReleaseTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<ClaimManager> Manager;

    protected override Status OnStart()
    {
        // TODO: We don't have any OnEnd event node to call this from automatically. Reservations may be leaked
        // if a graph is interrupted
        Manager.Value.Release(Target.Value);
        Target.Value = null;
        return Status.Success;
    }
}

