using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WithClaim", story: "With a [Target] claimed from [ClaimManager]", category: "Flow", id: "c2fc57bdb484d351adaa2b6856a8798d")]
public partial class WithClaimModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<ClaimManager> ClaimManager;

    protected override Status OnStart()
    {
        if (ClaimManager.Value == null) return Status.Failure;
        Target.Value = ClaimManager.Value.Claim().gameObject;
        if (Target.Value == null) return Status.Failure;

        return StartNode(Child);
    }

    protected override Status OnUpdate()
    {
        return Child.CurrentStatus;
    }

    protected override void OnEnd()
    {
    }
}

