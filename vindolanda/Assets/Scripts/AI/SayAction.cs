using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Vindolanda.Quest;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Say", story: "[Agent] says [Line]", category: "Action", id: "f93e3ce2b6677c9514a197b4e514e355")]
public partial class SayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Dialogue> Line;
    Speaker speaker;

    protected override Status OnStart()
    {
        speaker = Agent.Value.GetComponent<Speaker>();
        if (speaker == null)
        {
            Debug.LogError($"Speaker {Agent.Value.name} missing required Speaker component");
            return Status.Failure;
        }

        speaker.Say(Line.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (speaker.CurrentDialogue == null)
            return Status.Success;
        if (speaker.CurrentDialogue == Line.Value)
            return Status.Running;
        return Status.Failure; // Interrupted
    }
}

