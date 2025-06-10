using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntil", story: "Wait until [Variable] is true", category: "Action/Conditional", id: "5461355d011301632134a2bcd7f1dcab")]
public partial class WaitUntilAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;

    protected override Status OnStart()
    {
        return CheckStatus();
    }

    protected override Status OnUpdate()
    {
        return CheckStatus();
    }

    Status CheckStatus()
    {
        return Variable.Value ? Status.Success : Status.Running;
    }
}

