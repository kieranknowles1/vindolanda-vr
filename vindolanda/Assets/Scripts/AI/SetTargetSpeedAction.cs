using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetTargetSpeed", story: "Set [Output] based on [Agent] distance to [Target]", category: "Action", id: "c6d3c85249d5d6d40edca16a5130d086")]
public partial class SetTargetSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Output;

    [Tooltip("When at this distance or below, walk")]
    [SerializeReference] public BlackboardVariable<float> WalkAtDistance = new(7.0f);
    [Tooltip("Run at full speed if above this distance. If between this and WalkAtDistance, blend between walk and run")]
    [SerializeReference] public BlackboardVariable<float> RunAtDistance = new(20.0f);
    [SerializeReference] public BlackboardVariable<float> WalkSpeed = new(1.0f);
    [SerializeReference] public BlackboardVariable<float> RunSpeed = new(4.0f);

    protected override Status OnStart()
    {
        float distance = Agent.Value.transform.GetDistance(Target.Value.transform);

        Output.Value = CalculateTargetSpeed(distance);
        return Status.Success;
    }

    float CalculateTargetSpeed(float distance)
    {
        if (distance < WalkAtDistance.Value) return WalkSpeed.Value;
        if (distance > RunAtDistance.Value) return RunSpeed.Value;

        float blend = (distance - WalkAtDistance.Value) / (RunAtDistance.Value - WalkAtDistance.Value);
        return Mathf.Lerp(WalkSpeed.Value, RunSpeed.Value, blend);
    }
}

