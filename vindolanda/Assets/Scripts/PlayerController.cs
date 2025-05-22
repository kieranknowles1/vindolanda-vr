using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TunnelingVignetteController vignette;
    [SerializeField] private ControllerInputActionManager leftController;
    [SerializeField] private ControllerInputActionManager rightController;
    [SerializeField] private DynamicMoveProvider dynamicMoveProvider;

    private void UpdateSettings(GameSettings settings) {
        UpdateMovementType(settings.Movement.Type);
        vignette.defaultParameters.apertureSize = 1.0f - settings.Movement.VignetteStrength;
    }

    private void Start()
    {
        GameSettings.Instance.OnChange += UpdateSettings;
        UpdateSettings(GameSettings.Instance);
    }

    private void OnDestroy()
    {
        GameSettings.Instance.OnChange -= UpdateSettings;
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
}
