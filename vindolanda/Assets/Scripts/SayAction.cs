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
    Speaker speaker;
    Status status;

    void OnComplete(Dialogue dialogue, Speaker.SpeechResult result)
    {
        if (dialogue != Line.Value) return;
        status = result == Speaker.SpeechResult.Success ? Status.Success : Status.Failure;
    }

    protected override Status OnStart()
    {
        speaker = Agent.Value.GetComponent<Speaker>();
        if (speaker == null)
        {
            Debug.LogError($"Speaker {Agent.Value.name} missing required Speaker component");
            return Status.Failure;
        }

        speaker.Say(Line.Value);
        speaker.OnSpeechComplete += OnComplete;
        status = Status.Running;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return status;
    }

    protected override void OnEnd()
    {
        speaker.OnSpeechComplete -= OnComplete;
    }
}

