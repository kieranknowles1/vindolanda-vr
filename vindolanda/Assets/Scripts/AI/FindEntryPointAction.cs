using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindEntryPoint", story: "Find [EntryPoint] of [Target]", category: "Action/Find", id: "28df00dba8e7ef3b6b8128b7a7e32a8c")]
public partial class FindEntryPointAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> EntryPoint;
    [SerializeReference] public BlackboardVariable<Furniture> Target;

    protected override Status OnStart()
    {
        EntryPoint.Value = Target.Value.entryPoint.transform;
        return EntryPoint.Value != null ? Status.Success : Status.Failure;
    }
}

