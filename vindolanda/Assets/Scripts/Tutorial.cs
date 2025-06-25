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


    public Objective moveToTarget;
    public GameObject locomotionHints;

    public Dialogue intro;
    public Dialogue moveSmoothHint;

    Controller QuestController => GameConstants.Instance.QuestController;

    Coroutine routine;
    public void BeginTutorial()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RunTutorialAsync());
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

    IEnumerator RunTutorialAsync()
    {
        QuestController.GetState(tutorial).CurrentObjective = moveToTarget;
        locomotionHints.SetActive(true);

        yield return speaker.Say(intro);

        SayRepeatable(moveSmoothHint);
    }
}
