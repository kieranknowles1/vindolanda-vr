using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(HapticImpulsePlayer))]
public class ControllerEffects : MonoBehaviour
{
    [Flags]
    public enum ControllerButton
    {
        None = 0,
        Grip = 1 << 0,
        Trigger = 1 << 1,
        Stick = 1 << 2,
        A = 1 << 3,
        B = 1 << 4,

        All = Grip | Trigger | Stick | A | B
    }

    public Material glow;
    Material originalMaterial;

    public MeshRenderer grip;
    public MeshRenderer trigger;
    public MeshRenderer stick;
    public MeshRenderer buttonA;
    public MeshRenderer buttonB;

    HapticImpulsePlayer haptics;

    ControllerButton glowState;
    public ControllerButton GlowState
    {
        get => glowState;
        set
        {
            var old = glowState;
            var clamped = value & ControllerButton.All;
            glowState = clamped;
            UpdateGlow(clamped, grip, ControllerButton.Grip);
            UpdateGlow(clamped, trigger, ControllerButton.Trigger);
            UpdateGlow(clamped, stick, ControllerButton.Stick);
            UpdateGlow(clamped, buttonA, ControllerButton.A);
            UpdateGlow(clamped, buttonB, ControllerButton.B);

            // Send haptics if one or more glows were enabled
            // ~old & ControllerButton.All represents all buttons that were not previously highlighted
            if ((clamped & (~old & ControllerButton.All)) != 0) {
                // An untracked controller is disabled, and can't be given haptics or coroutines
                if (gameObject.activeInHierarchy)
                    StartCoroutine(SendHaptics());
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalMaterial = grip.sharedMaterial;
        haptics = GetComponent<HapticImpulsePlayer>();
    }

    IEnumerator SendHaptics()
    {
        float pulseDuration = 0.05f;
        haptics.SendHapticImpulse(.25f, pulseDuration);
        yield return new WaitForSeconds(pulseDuration + 0.1f);
        haptics.SendHapticImpulse(.25f, pulseDuration);
    }

    void UpdateGlow(ControllerButton state, MeshRenderer button, ControllerButton mask)
    {
        bool lit = state.HasFlag(mask);
        button.sharedMaterial = lit ? glow : originalMaterial;
    }
}
