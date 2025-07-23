using UnityEngine;
using Vindolanda.Quest;

public class SetTourDialogue : TriggerBase
{
    public DialogueEvent setTourDialogue;
    public Dialogue dialogue;

    protected override void Execute(PlayerController player)
    {
        setTourDialogue.SendEventMessage(dialogue);

        if (GameConstants.Instance.Tour.GuideFollowing)
        {
            player.LeftControllerEffects.GlowState |= ControllerEffects.ControllerButton.A;
            player.RightControllerEffects.GlowState |= ControllerEffects.ControllerButton.A;
        }
    }

    protected override void ExecuteExit(PlayerController player)
    {
        setTourDialogue.SendEventMessage(null);

        if (GameConstants.Instance.Tour.GuideFollowing)
        {
            player.LeftControllerEffects.GlowState &= ~ControllerEffects.ControllerButton.A;
            player.RightControllerEffects.GlowState &= ~ControllerEffects.ControllerButton.A;
        }
    }
}
