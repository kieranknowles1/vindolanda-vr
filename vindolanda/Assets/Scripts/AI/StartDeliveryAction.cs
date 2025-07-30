using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartDelivery", story: "[Agent] gives a delivery quest", category: "Action", id: "77f2c33c141b0f1c98faacd6be9abd20")]
public partial class StartDeliveryAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        GameConstants.Instance.DeliveryController.StartDelivery(Agent.Value.GetComponent<ActorController>());
        return Status.Success;
    }
}

