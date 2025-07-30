using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DialogueChoice", story: "[Actor] prompts player with choice from list.", category: "Flow", id: "2227d31e2da40dc7b2da14c2302ea4ea")]
public partial class DialogueChoiceSequence : Composite
{
    [SerializeReference] public BlackboardVariable<Speaker> Actor;
    PlayerController Player => GameConstants.Instance.Player;
    [SerializeReference] public Node Option1;
    [SerializeReference] public BlackboardVariable<DialogueChoices> Choices;
    [SerializeReference] public Node Option2;

    [Tooltip("Fail if the player walks further than this distance away.")]
    [SerializeReference] public BlackboardVariable<float> MaxPlayerDistance = new(10.0f);

    DialogueMenu menu;
    Node activeChild = null;

    List<string> ResolveOptions()
    {
        List<string> result = new();
        void Resolve(LocalizedString key)
        {
            if (key == null) return;
            result.Add(key.GetLocalizedString());
        }
        Resolve(Choices.Value.positive);
        Resolve(Choices.Value.negative);
        return result;
    }

    void OnClicked(int i)
    {
        activeChild = i switch
        {
            0 => Option1,
            1 => Option2,
            _ => throw new UnreachableException()
        };

        if (activeChild == null)
        {
            LogFailure("Child is null", true);
            return;
        }
        DisableMenu();
        StartNode(activeChild);
    }

    protected override Status OnStart()
    {
        menu = Player.ShowDialogueMenu(Actor.Value, ResolveOptions());
        menu.onClicked.AddListener(OnClicked);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Player.transform.GetDistance(Actor.Value.transform) > MaxPlayerDistance.Value)
            return Status.Failure;

        if (activeChild == null) return Status.Running;

        return activeChild.CurrentStatus;
    }

    void DisableMenu()
    {
        menu.onClicked.RemoveListener(OnClicked);
        Player.CloseDialogueMenu();
    }

    protected override void OnEnd()
    {
        DisableMenu();
    }
}

