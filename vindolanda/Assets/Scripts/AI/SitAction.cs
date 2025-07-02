using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sit", story: "[Agent] [Action] on [Target]", category: "Action", id: "1120f4987b6494902140919b252bb978")]
public partial class SitAction : Action
{
    public enum SitStand
    {
        Sit,
        StandUp
    }

    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<SitStand> Action;
    [SerializeReference] public BlackboardVariable<Furniture> Target;

    ActorController actor;
    NavMeshAgent navAgent;
    Status status = Status.Running;

    protected override Status OnStart()
    {
        actor = Agent.Value.GetComponent<ActorController>();
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (actor == null) return Status.Failure;

        // Don't snap to the navmesh, prevents issues with clipping
        navAgent.enabled = Action.Value == SitStand.StandUp;

        if (Action.Value == SitStand.Sit)
            actor.StartCoroutine(Target.Value.Sit(actor, OnSitDone));
        else
            actor.StartCoroutine(Target.Value.Stand(actor, OnSitDone));

        // May fail immediatly and trigger callback
        return status;
    }

    void OnSitDone(Furniture.SitResult result)
    {
        status = result == Furniture.SitResult.Success ? Status.Success : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return status;
    }

    protected override void OnEnd()
    {
    }
}

