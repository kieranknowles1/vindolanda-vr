using System;
using UnityEngine;

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

    ControllerButton glowState;
    public ControllerButton GlowState
    {
        get => glowState;
        set
        {
            glowState = value;
            UpdateGlow(value, grip, ControllerButton.Grip);
            UpdateGlow(value, trigger, ControllerButton.Trigger);
            UpdateGlow(value, stick, ControllerButton.Stick);
            UpdateGlow(value, buttonA, ControllerButton.A);
            UpdateGlow(value, buttonB, ControllerButton.B);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalMaterial = grip.sharedMaterial;
    }

    void UpdateGlow(ControllerButton state, MeshRenderer button, ControllerButton mask)
    {
        bool lit = state.HasFlag(mask);
        button.sharedMaterial = lit ? glow : originalMaterial;
    }
}
