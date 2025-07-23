using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private TunnelingVignetteController vignette;

    public ControllerInputActionManager leftController;
    public ControllerEffects LeftControllerEffects { get; private set; }

    public ControllerInputActionManager rightController;
    public ControllerEffects RightControllerEffects { get; private set; }

    [SerializeField]
    private DynamicMoveProvider dynamicMoveProvider;

    public SubtitlePanel Subtitles;

    [SerializeField] DialogueMenu dialogueMenu;

    InputActions input;

    public GameObject settingsMenu;
    [Tooltip("Position of headset")]
    public Transform head;

    private void UpdateSettings(GameSettings settings)
    {
        UpdateMovementType(settings.Movement.Type);
        vignette.defaultParameters.apertureSize = 1.0f - settings.Movement.VignetteStrength;
    }

    private void Start()
    {
        input = new InputActions();
        GameConstants.Instance.Player = this;
        GameSettings.Instance.OnChange += UpdateSettings;
        UpdateSettings(GameSettings.Instance);

        input.GameInputs.Enable();
        input.GameInputs.ToggleMenu.performed += ToggleSettings;

        LeftControllerEffects = leftController.GetComponent<ControllerEffects>();
        RightControllerEffects = rightController.GetComponent<ControllerEffects>();
    }

    private void OnDestroy()
    {
        GameSettings.Instance.OnChange -= UpdateSettings;
        input.GameInputs.ToggleMenu.performed -= ToggleSettings;
    }

    private void UpdateMovementType(GameSettings.MovementType type)
    {
        // TODO: Maybe allow selecting controller used for movement
        var movementController = rightController;

        // TODO: teleportEnabled field instead of using !smoothMotionEnabled
        movementController.smoothMotionEnabled = type != GameSettings.MovementType.Teleport;

        switch (type)
        {
            case GameSettings.MovementType.Teleport:
                break;
            case GameSettings.MovementType.SmoothHeadForward:
                dynamicMoveProvider.forwardSource = mainCamera.transform;
                break;
            case GameSettings.MovementType.SmoothControllerForward:
                dynamicMoveProvider.forwardSource = movementController.transform;
                break;
        }
    }

    void ToggleSettings(InputAction.CallbackContext _)
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);

        var planeAngle = Quaternion.AngleAxis(head.eulerAngles.y, Vector3.up);

        settingsMenu.transform.SetPositionAndRotation(
            head.position + (planeAngle * Vector3.forward * 1.5f),
            head.rotation = planeAngle
        );
    }

    // Preferred distance between speaker and dialogue prompts
    const float PreferredDialogueDistance = 1.0f;
    // Minimum distance between speaker and dialogue prompts, in fraction of total distance between speaker and player
    // Used if our first choice would be too close
    const float MinDialogueRatio = 0.3f;

    void CalculateDialogueMenuPosition(Transform speaker, out Vector3 position, out Quaternion rotation)
    {
        // Keep everything at the same height, assumes we're not on any stairs
        Vector2 speakXz = new(speaker.position.x, speaker.position.z);
        Vector2 playerXz = new(head.position.x, head.position.z);

        Vector2 speakerToPlayer = (speakXz - speakXz);;

        Vector2 finalXz;
        if (speakerToPlayer.magnitude > (PreferredDialogueDistance * MinDialogueRatio))
        { // We're far enough to reach our preferred distance
            finalXz = speakXz + (MinDialogueRatio * speakerToPlayer.normalized);
        }
        else
        { // PreferredDialogueDistance would be too close
            finalXz = Vector2.Lerp(speakXz, playerXz, MinDialogueRatio);
        }

        // Slightly below eye level
        position = new(finalXz.x, head.position.y - 0.6f, finalXz.y);
        rotation = Quaternion.LookRotation(new(speakerToPlayer.x, 0, speakerToPlayer.y), Vector3.up);
    }

    public DialogueMenu ShowDialogueMenu(Speaker speaker, List<string> options)
    {
        dialogueMenu.Display(speaker.ActorName.GetLocalizedString(), options);

        // Position the menu facing the player and near the speaker
        var speakerToPlayer = Vector3.Distance(speaker.transform.position, head.position);
        Vector3 menuPos;
        // But keep it on the same Y plane, assumes we're not on stairs
        var speakerDirection = speaker.transform.position - head.position;
        speakerDirection.y = 0;
        speakerDirection = speakerDirection.normalized;
        if (speakerToPlayer < PreferredDialogueDistance)
        {
            menuPos = Vector3.Lerp(speaker.transform.position, head.position, PreferredDialogueDistance);
        }
        else
        {
            menuPos = speaker.transform.position + (speakerDirection * PreferredDialogueDistance);
        }
        menuPos.y = head.position.y;

        // Face the player
        CalculateDialogueMenuPosition(speaker.transform, out var pos, out var rot);
        dialogueMenu.transform.SetPositionAndRotation(pos, rot);
        dialogueMenu.gameObject.SetActive(true);
        return dialogueMenu;
    }

    public void CloseDialogueMenu()
    {
        dialogueMenu.Clear();
        dialogueMenu.gameObject.SetActive(false);
    }
}
