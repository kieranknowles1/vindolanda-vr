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

    [Header("Movement")]
    [SerializeField] private MovementType moveType;
    public MovementType MoveType
    {
        get => moveType;
        set
        {
            moveType = value;
            UpdateMovementType(value);
        }
    }

    [SerializeField] private float vignetteStrength;
    public float VignetteStrength
    {
        get => vignetteStrength;
        set
        {
            vignetteStrength = value;
            vignette.defaultParameters.apertureSize = 1.0f - value;
        }
    }

    private void Start()
    {
        // Force game state to match defaults given here
        MoveType = moveType;
        VignetteStrength = vignetteStrength;
    }

    private void UpdateMovementType(MovementType type)
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
