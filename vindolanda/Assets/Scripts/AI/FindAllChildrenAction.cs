using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.XR.CoreUtils;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindAllChildren", story: "Find all [Children] of [Root]", category: "Action", id: "78afab8860c5f4a255947a734083cee1")]
public partial class FindAllChildrenAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Children;
    [SerializeReference] public BlackboardVariable<GameObject> Root;

    protected override Status OnStart()
    {
        if (Root.Value == null) return Status.Failure;
        if (Children.Value == null) return Status.Failure;
        Root.Value.GetChildGameObjects(Children.Value);
        return Status.Success;
    }
}

