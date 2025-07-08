using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetTargetSpeed", story: "Set [Agent] speed based on distance to [Target]", category: "Action", id: "c6d3c85249d5d6d40edca16a5130d086")]
public partial class SetTargetSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [Tooltip("When at this distance or below, walk")]
    [SerializeReference] public BlackboardVariable<float> WalkAtDistance = new(7.0f);
    [Tooltip("Run at full speed if above this distance. If between this and WalkAtDistance, blend between walk and run")]
    [SerializeReference] public BlackboardVariable<float> RunAtDistance = new(20.0f);
    [SerializeReference] public BlackboardVariable<float> WalkSpeed = new(1.0f);
    [SerializeReference] public BlackboardVariable<float> RunSpeed = new(4.0f);

    [Tooltip("Duration to smooth speed update over. If zero, run instantly")]
    [SerializeReference] public BlackboardVariable<float> TransitionDuration = new(0.5f);

    NavMeshAgent navAgent;
    float startSpeed;
    float endSpeed;
    float elapsed;

    protected override Status OnStart()
    {
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        float distance = Agent.Value.transform.GetDistance(Target.Value.transform);
        startSpeed = navAgent.speed;
        endSpeed = CalculateTargetSpeed(distance);
        elapsed = 0;

        if (TransitionDuration.Value == 0)
        {
            OnUpdate();
            return Status.Running;
        }
        else
        {
            return Status.Running;
        }
    }

    protected override Status OnUpdate()
    {
        elapsed += Time.deltaTime;
        float ratio = TransitionDuration.Value > 0 ? elapsed / TransitionDuration.Value : 1.0f;
        navAgent.speed = Mathf.Lerp(startSpeed, endSpeed, ratio);
        return elapsed >= TransitionDuration.Value ? Status.Success : Status.Running;
    }

    float CalculateTargetSpeed(float distance)
    {
        if (distance < WalkAtDistance.Value) return WalkSpeed.Value;
        if (distance > RunAtDistance.Value) return RunSpeed.Value;

        float blend = (distance - WalkAtDistance.Value) / (RunAtDistance.Value - WalkAtDistance.Value);
        return Mathf.Lerp(WalkSpeed.Value, RunSpeed.Value, blend);
    }
}

