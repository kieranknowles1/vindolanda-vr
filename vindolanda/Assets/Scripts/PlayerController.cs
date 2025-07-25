using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// An object the player can speak to
/// If multiple exist, the closest will be picked
/// </summary>
public interface ISpeechListener
{
    /// <summary>
    /// Can the player speak to this object at the moment?
    /// </summary>
    bool PlayerCanSpeakTo { get; }
    /// <summary>
    /// Will this have priority over all other targets?
    /// Order is unspecified if multiple exist
    /// </summary>
    bool ForceSpeak { get; }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Unity builtin")]
    Transform transform { get; }

    void Speak(PlayerController player);
}

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

    public InputActionReference toggleSettings;
    public InputActionReference speak;

    public GameObject settingsMenu;
    [Tooltip("Position of headset")]
    public Transform head;

    private void OnSettingsChanged(GameSettings settings)
    {
        UpdateMovementType(settings.Movement.Type);
        vignette.defaultParameters.apertureSize = 1.0f - settings.Movement.VignetteStrength;

        LeftControllerEffects.visuals.SetActive(settings.ShowControllers);
        RightControllerEffects.visuals.SetActive(settings.ShowControllers);
    }

    private void Start()
    {
        LeftControllerEffects = leftController.GetComponent<ControllerEffects>();
        RightControllerEffects = rightController.GetComponent<ControllerEffects>();

        GameSettings.Instance.OnChange += OnSettingsChanged;
        OnSettingsChanged(GameSettings.Instance);

        toggleSettings.action.performed += ToggleSettings;
        speak.action.performed += Speak;
    }

    private void OnDestroy()
    {
        GameSettings.Instance.OnChange -= OnSettingsChanged;
        toggleSettings.action.performed -= ToggleSettings;
        speak.action.performed -= Speak;
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

    #region Dialogue

    public SubtitlePanel Subtitles;

    [SerializeField] DialogueMenu dialogueMenu;

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

    public readonly List<ISpeechListener> speechTargets = new();

    void Speak(InputAction.CallbackContext _)
    {
        ISpeechListener best = null;
        foreach (var target in speechTargets)
        {
            if (!target.PlayerCanSpeakTo) continue;
            if (target.ForceSpeak)
            {
                best = target;
                break;
            }

            if (best == null)
            {
                best = target;
            }
            else
            {
                var bestDistance = Vector3.Distance(transform.position, best.transform.position);
                var thisDistance = Vector3.Distance(transform.position, target.transform.position);
                if (thisDistance < bestDistance)
                {
                    best = target;
                }
            }
        }

        best?.Speak(this);
    }

    #endregion
}
