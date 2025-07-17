using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WithFollower", story: "With [Self] as follower of [Target]", category: "Flow", id: "0ef70f59f907422c2ed6f511739b5fdb")]
public partial class WithFollowerModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        var self = Self.Value.GetComponent<ActorController>();
        if (Target.Value.TryGetComponent<FollowerTracker>(out var follow))
        {
            follow.followers.Add(self);
        }
        return StartNode(Child);
    }

    protected override Status OnUpdate()
    {
        return Child.CurrentStatus;
    }

    protected override void OnEnd()
    {
        var self = Self.Value.GetComponent<ActorController>();
        if (Target.Value != null && Target.Value.TryGetComponent<FollowerTracker>(out var follow))
        {
            follow.followers.Remove(self);
        }
    }
}

