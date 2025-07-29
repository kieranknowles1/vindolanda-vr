using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Vindolanda.Quest;

[RequireComponent(typeof(Speaker))]
public class Tutorial : MonoBehaviour, ISpeechListener
{
    bool active = false;
    public bool PlayerCanSpeakTo(PlayerController _) => active;
    public bool ForceSpeak => true;

    public void Speak(PlayerController player)
    {
        RepeatInstruction();
    }

    Speaker speaker;

    private void Awake()
    {
        speaker = GetComponent<Speaker>();
        GameConstants.Instance.Player.speechTargets.Add(this);
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
        public Dialogue dialMoveTeleport;
    }
    public LocomotionStage locomotion;

    Quest.State QuestState => GameConstants.Instance.QuestController.GetState(tutorial);

    public void BeginTutorial()
    {
        active = true;
        GameConstants.Instance.Player.transform.Teleport(locomotion.startMarker);
        GameConstants.Instance.Player.settingsMenu.SetActive(false);
        QuestState.CurrentObjective = locomotion.moveToTarget;
        locomotion.tutorialObjects.SetActive(true);

        IEnumerator SayIntro()
        {
            yield return speaker.Say(locomotion.dialTutorialInfo);
            if (GameSettings.Instance.Movement.Type == GameSettings.MovementType.Teleport)
            {
                yield return SayRepeatable(locomotion.dialMoveTeleport);
            }
            else // smooth movement
            {
                yield return SayRepeatable(locomotion.dialMoveSmooth);
            }
            GameConstants.Instance.Player.RightControllerEffects.GlowState |= ControllerEffects.ControllerButton.Stick;
        }
        StartCoroutine(SayIntro());
    }

    Dialogue currentDialogue;
    Coroutine SayRepeatable(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        return speaker.Say(dialogue);
    }

    void RepeatInstruction()
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
        GameConstants.Instance.Player.RightControllerEffects.GlowState &= ~ControllerEffects.ControllerButton.Stick;

        locomotion.tutorialObjects.SetActive(false);
        interaction.tutorialObjects.SetActive(true);
        QuestState.CurrentObjective = interaction.pickUpItem;
        SayRepeatable(interaction.dialPickUp);
    }

    [Serializable]
    public struct Menus
    {
        public Objective objectiveSaveGame;
        public GameObject tutorialObjects;

        public SaveLoadMenu saveLoadMenu;

        public ConfirmButton saveGame;
        public Dialogue dialStart;
        public Dialogue dialOpenMenu;
        public Dialogue dialClickSave;
        public Dialogue dialCloseMenu;
    }
    public Menus menus;

    bool gameSaved = false;
    void OnMenuOpenClose(bool enable)
    {
        if (enable)
        {
            // Opened menu and have yet to save game
            if (!gameSaved)
            {
                SayRepeatable(menus.dialClickSave);
            }
        }
        else
        {
            // Closed menu after saving game
            if (gameSaved)
            {
                Evnt_MenusDone();
            }
            // Closed menu before saving game, repeat instruction to open it
            else
            {
                SayRepeatable(menus.dialOpenMenu);
            }
        }
    }

    void OnButtonClicked()
    {
        gameSaved = true;
        SayRepeatable(menus.dialCloseMenu);
    }

    public void Evnt_ItemHeld()
    {
        gameSaved = false;
        QuestState.CurrentObjective = menus.objectiveSaveGame;
        IEnumerator StartMessage() {
            yield return speaker.Say(menus.dialStart);
            interaction.tutorialObjects.SetActive(false);
            menus.tutorialObjects.SetActive(true);
            SayRepeatable(menus.dialOpenMenu);
        }
        StartCoroutine(StartMessage());

        menus.saveLoadMenu.onEnableStateChange.AddListener(OnMenuOpenClose);
        menus.saveGame.onConfirm.AddListener(OnButtonClicked);
    }

    void Evnt_ItemHeldCleanup()
    {
        menus.saveLoadMenu.onEnableStateChange.RemoveListener(OnMenuOpenClose);
        menus.saveGame.onConfirm.RemoveListener(OnButtonClicked);
    }

    [Serializable]
    public struct Finale
    {
        public Dialogue dialComplete;
    }
    public Finale finale;

    public void Evnt_MenusDone()
    {
        Evnt_ItemHeldCleanup();
        QuestState.Complete = true;
        currentDialogue = null;
        menus.tutorialObjects.SetActive(false);
        speaker.Say(finale.dialComplete);

        active = false;
    }
}
