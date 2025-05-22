using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

/// <summary>
/// Very simple and very limited keyboard input for an XR controller. Only intended for testing
/// without a headset connected. Don't expect much more than the bare minimum.
/// </summary>
public class KeyboardMouseProvider : LocomotionProvider
{
    public float MoveSpeed = 4.0f;
    public float LookSensitivity = 0.3f;
    public GameObject ForwardSource;

    InputActions input;
    readonly XROriginMovement translation = new();
    readonly XRBodyYawRotation rotation = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = new InputActions();
        input.Keyboard.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        var move = input.Keyboard.Movement.ReadValue<Vector2>();
        if (move != Vector2.zero)
        {
            Vector3 moveVec = new Vector3(move.x, 0, move.y);
            translation.motion = ForwardSource.transform.rotation * moveVec * MoveSpeed * Time.deltaTime;
            TryStartLocomotionImmediately();
            TryQueueTransformation(translation);
        }

        var look = input.Keyboard.LockLook.IsPressed() ? Vector2.zero : input.Keyboard.Look.ReadValue<Vector2>();
        if (look != Vector2.zero)
        {
            // Default VR input doesn't support rotation since we don't have artificial gravity (yet)
            rotation.angleDelta = look.x * LookSensitivity;
            TryStartLocomotionImmediately();
            TryQueueTransformation(rotation);
        }
    }
}
