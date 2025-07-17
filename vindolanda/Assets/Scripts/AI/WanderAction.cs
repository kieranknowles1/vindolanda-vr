using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[Agent] wanders within [Radius] metres of [WanderMode]", category: "Action/Navigation", id: "f153eb1f5e7e08a5ab06cdc9d9b5b010")]
public partial class WanderAction : Action
{
    public enum WanderType
    {
        CurrentPosition,
        StartPosition
    }

    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<WanderType> WanderMode;

    private NavMeshAgent navAgent;
    private ActorController controller;
    [CreateProperty] private Vector3 target;

    protected override Status OnStart()
    {
        if (!Initialise()) return Status.Failure;
        bool ok = NavUtil.RandomNavmeshPosition(GetWanderOrigin(), Radius.Value, out target);
        if (!ok) return Status.Failure;

        navAgent.SetDestination(target);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return NavUtil.ReachedDestination(navAgent) ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }

    protected override void OnDeserialize()
    {
        base.OnDeserialize();
        Initialise();
    }

    Vector3 GetWanderOrigin()
    {
        return WanderMode.Value switch
        {
            WanderType.CurrentPosition => controller.transform.position,
            WanderType.StartPosition => controller.OriginalPosition,
            _ => throw new UnreachableException()
        };
    }

    bool Initialise()
    {
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        controller = Agent.Value.GetComponent<ActorController>();

        return navAgent != null && controller != null;
    }
}

