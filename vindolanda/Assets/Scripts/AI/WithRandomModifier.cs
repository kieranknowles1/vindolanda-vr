using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Random = UnityEngine.Random;
using Unity.Properties;
using Unity.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WithRandom", story: "With random [Item] from [Collection]", description: "Run children with a random item which is removed from the collection", category: "Flow", id: "f681bdbf4db75e6d8864fb15f01c9281")]
public partial class WithRandomModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<GameObject> Item;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Collection;

    protected override Status OnStart()
    {
        int index = Random.Range(0, Collection.Value.Count);
        Item.Value = Collection.Value[index];
        Collection.Value.RemoveAtSwapBack(index);
        return StartNode(Child);
    }

    protected override Status OnUpdate()
    {
        return Child.CurrentStatus;
    }

    protected override void OnEnd()
    {
        Collection.Value.Add(Item.Value);
    }
}

