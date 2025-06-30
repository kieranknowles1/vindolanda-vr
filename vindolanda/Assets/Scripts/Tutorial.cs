using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Vindolanda.Quest;

[RequireComponent(typeof(Speaker))]
public class Tutorial : MonoBehaviour
{
    InputActions input;
    Speaker speaker;

    private void Awake()
    {
        input = new InputActions();
        speaker = GetComponent<Speaker>();

        input.GameInputs.Enable();
        input.GameInputs.TutorialRepeatInstruction.performed += RepeatInstruction;
    }

    private void OnDestroy()
    {
        input.GameInputs.TutorialRepeatInstruction.performed -= RepeatInstruction;
    }

    public Quest tutorial;

    [Serializable]
    public struct LocomotionStage
    {
        public Transform startMarker;
        public Objective moveToTarget;
        public GameObject tutorialObjects;

        public Dialogue dialTutorialInfo;
        public Dialogue dialMoveSmooth;
    }
    public LocomotionStage locomotion;

    Quest.State QuestState => GameConstants.Instance.QuestController.GetState(tutorial);

    public void BeginTutorial()
    {
        GameConstants.Instance.Player.Teleport(locomotion.startMarker);
        GameConstants.Instance.Player.settingsMenu.SetActive(false);
        QuestState.CurrentObjective = locomotion.moveToTarget;
        locomotion.tutorialObjects.SetActive(true);

        IEnumerator SayIntro()
        {
            yield return speaker.Say(locomotion.dialTutorialInfo);
            SayRepeatable(locomotion.dialMoveSmooth);
        }
        StartCoroutine(SayIntro());
    }

    Dialogue currentDialogue;
    void SayRepeatable(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        speaker.Say(dialogue);
    }

    void RepeatInstruction(InputAction.CallbackContext _)
    {
        if (currentDialogue != null)
        {
            speaker.Say(currentDialogue);
        }
    }

    [Serializable]
    public struct ItemInteraction
    {
        public GameObject tutorialObjects;
        public Objective pickUpItem;
        public Dialogue dialPickUp;
    }
    public ItemInteraction interaction;

    public void Evnt_LocomotionTargetReached()
    {
        locomotion.tutorialObjects.SetActive(false);
        interaction.tutorialObjects.SetActive(true);
        QuestState.CurrentObjective = interaction.pickUpItem;
        SayRepeatable(interaction.dialPickUp);

        // TODO: Play mocap animation of picking up an item
    }

    [Serializable]
    public struct Finale
    {
        public Dialogue dialComplete;
    }
    public Finale finale;

    public void Evnt_ItemHeld()
    {
        
        QuestState.Complete = true;
        currentDialogue = null;
        // TODO: Prompt user to open the menu and save their game
        IEnumerator SayAsync()
        {
            yield return speaker.Say(finale.dialComplete);
            interaction.tutorialObjects.SetActive(false);
        }
        StartCoroutine(SayAsync());
    }
}
