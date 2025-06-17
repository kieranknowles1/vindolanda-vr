using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindGameObject", story: "Find [Object] with ID [Id]", category: "Action/Find", id: "fe719310804e1c1e0768520767eddfec")]
public partial class FindGameObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GuidComponent> Object;
    [SerializeReference] public BlackboardVariable<int> Id;

    protected override Status OnStart()
    {
        Object.Value = GuidManager.Instance.TryFind<GuidComponent>(Id.Value);
        return Object.Value != null ? Status.Success : Status.Failure;
    }
}

