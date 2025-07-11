using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Vindolanda.Quest;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Linq;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SayMany", story: "[Agents] say [Line]", category: "Action", id: "01ea6d6d96143df3ba0c4e064df077d2")]
public partial class SayManyAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Agents;
    [SerializeReference] public BlackboardVariable<Dialogue> Line;
    [SerializeReference] public BlackboardVariable<GameObject> Exception;

    List<Speaker> speakers;

    protected override Status OnStart()
    {
        speakers = Agents.Value
            .Where(a => a != Exception.Value)
            .Select(a => a.GetComponent<Speaker>())
            .NotNull().ToList();

        foreach (var speaker in speakers)
        {
            speaker.Say(Line.Value);
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return speakers.All(s => s.CurrentDialogue != Line.Value) ? Status.Success : Status.Running;
    }
}

