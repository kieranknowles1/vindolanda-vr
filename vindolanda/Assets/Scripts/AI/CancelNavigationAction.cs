using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CancelNavigation", story: "[Self] cancels navigation.", category: "Action", id: "c655a28418ed052436796ea51a1691df")]
public partial class CancelNavigationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<NavMeshAgent>().isStopped = true;
        return Status.Success;
    }
}

