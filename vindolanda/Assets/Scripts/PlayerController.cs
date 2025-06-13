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

    [SerializeField]
    private ControllerInputActionManager leftController;

    [SerializeField]
    private ControllerInputActionManager rightController;

    [SerializeField]
    private DynamicMoveProvider dynamicMoveProvider;

    public SubtitlePanel Subtitles;

    InputActions input;

    [SerializeField] GameObject settingsMenu;
    [Tooltip("Position of headset")]
    [SerializeField] Transform head;

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

        settingsMenu.transform.position = head.position + (planeAngle * Vector3.forward * 1.5f);
        settingsMenu.transform.rotation = head.rotation = planeAngle;
    }
}
