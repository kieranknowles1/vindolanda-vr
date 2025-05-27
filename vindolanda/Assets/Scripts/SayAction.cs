using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Say", story: "[Agent] says [Line]", category: "Action", id: "f93e3ce2b6677c9514a197b4e514e355")]
public partial class SayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Dialogue> Line;
    int currentIndex = 0;

    protected override Status OnStart()
    {
        currentIndex = 0;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (currentIndex >= Line.Value.Lines.Count)
            return Status.Success;
        
        Debug.Log(Line.Value.Lines[currentIndex].Text);
        currentIndex++;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

