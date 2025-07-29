using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DialogueChoice", story: "[Actor] prompts [Player] with choice from list.", category: "Flow", id: "2227d31e2da40dc7b2da14c2302ea4ea")]
public partial class DialogueChoiceSequence : Composite
{
    [SerializeReference] public BlackboardVariable<Speaker> Actor;
    [SerializeReference] public BlackboardVariable<PlayerController> Player;
    [SerializeReference] public Node Option1;
    [SerializeReference] public BlackboardVariable<string> Choice1Key;
    [SerializeReference] public Node Option2;
    [SerializeReference] public BlackboardVariable<string> Choice2Key;

    DialogueMenu menu;
    Node activeChild = null;

    List<string> ResolveOptions()
    {
        List<string> result = new();
        void Resolve(string key)
        {
            if (key == null) return;
            result.Add(new LocalizedString(LocalizationSettings.StringDatabase.DefaultTable, key).GetLocalizedString());
        }
        Resolve(Choice1Key.Value);
        Resolve(Choice2Key.Value);
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
        menu = Player.Value.ShowDialogueMenu(Actor.Value, ResolveOptions());
        menu.onClicked.AddListener(OnClicked);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (activeChild == null) return Status.Running;

        return activeChild.CurrentStatus;
    }

    void DisableMenu()
    {
        menu.onClicked.RemoveListener(OnClicked);
        Player.Value.CloseDialogueMenu();
    }

    protected override void OnEnd()
    {
        DisableMenu();
    }
}

