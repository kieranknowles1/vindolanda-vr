using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour
{
    public enum MovementType
    {
        Teleport = 0,
        SmoothHeadForward = 1,
        SmoothControllerForward = 2
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private TunnelingVignetteController vignette;
    [SerializeField] private ControllerInputActionManager leftController;
    [SerializeField] private ControllerInputActionManager rightController;
    [SerializeField] private DynamicMoveProvider dynamicMoveProvider;

    public void SetVignetteStrength(float strength)
    {
        vignette.defaultParameters.apertureSize = 1.0f - strength;
    }

    public void SetMovementType(MovementType type)
    {
        // TODO: Maybe allow selecting controller used for movement
        var movementController = rightController;

        // TODO: teleportEnabled field instead of using !smoothMotionEnabled
        movementController.smoothMotionEnabled = type != MovementType.Teleport;

        switch (type)
        {
            case MovementType.Teleport:
                break;
            case MovementType.SmoothHeadForward:
                dynamicMoveProvider.forwardSource = mainCamera.transform;
                break;
            case MovementType.SmoothControllerForward:
                dynamicMoveProvider.forwardSource = movementController.transform;
                break;
        }
    }
}
