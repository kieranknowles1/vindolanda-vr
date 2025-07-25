using Unity.Behavior;
using UnityEngine;

public class TourController : MonoBehaviour, ISpeechListener
{
    public DefaultEvent trySayTourDialogue;

    public BehaviorGraphAgent agent;

    public bool GuideFollowing => agent.GetVariableValue<bool>("Following");

    public bool PlayerCanSpeakTo => true;
    public bool ForceSpeak => false;

    private void Awake()
    {
        GameConstants.Instance.Player.speechTargets.Add(this);
    }

    public void Speak(PlayerController player)
    {
        trySayTourDialogue.SendEventMessage();
    }
}
