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

    public Transform startMarker;

    public Quest tutorial;


    public Objective moveToTarget;

    public GameObject locomotionHints;
    public Dialogue intro;
    public Dialogue moveSmoothHint;

    public GameObject pickUpDemo;
    public Objective pickUpItem;
    public Dialogue pickUpPrompt;

    public Dialogue completeMessage;

    Quest.State QuestState => GameConstants.Instance.QuestController.GetState(tutorial);

    public void BeginTutorial()
    {
        GameConstants.Instance.Player.Teleport(startMarker);
        GameConstants.Instance.Player.settingsMenu.SetActive(false);
        QuestState.CurrentObjective = moveToTarget;
        locomotionHints.SetActive(true);

        IEnumerator SayIntro()
        {
            yield return speaker.Say(intro);
            SayRepeatable(moveSmoothHint);
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

    public void Evnt_LocomotionTargetReached()
    {
        locomotionHints.SetActive(false);
        pickUpDemo.SetActive(true);
        QuestState.CurrentObjective = pickUpItem;
        SayRepeatable(pickUpPrompt);

        // TODO: Play mocap animation of picking up an item
    }

    public void Evnt_ItemHeld()
    {
        
        QuestState.Complete = true;
        currentDialogue = null;
        // TODO: Prompt user to open the menu and save their game
        IEnumerator SayAsync()
        {
            yield return speaker.Say(completeMessage);
            pickUpDemo.SetActive(false);
        }
        StartCoroutine(SayAsync());
    }
}
