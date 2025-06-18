using System;
using UnityEngine;

[Serializable]
struct ReferencePoint
{
    public Transform start;
    public Transform end;
    public Transform current;

    public void LerpPosition(float factor) => current.position = Vector3.Lerp(start.position, end.position, factor);
}

[RequireComponent(typeof(MeshRenderer))]
public class BowAnimator : MonoBehaviour
{
    static readonly int CurrentFrameVar = Shader.PropertyToID("_frame");
    static readonly int TotalFramesVar = Shader.PropertyToID("_frames");

    [SerializeField, Range(0, 1)] float drawLevel;
    float totalFrames;

    float animOffset = 0.33f;

    [SerializeField] ReferencePoint connectTop;
    [SerializeField] ReferencePoint connectBottom;
    [SerializeField] ReferencePoint nock;
    [SerializeField] GameObject fakeArrow;

    public float DrawLevel
    {
        get => drawLevel;
        set {
            drawLevel = value;
            UpdateAnimation();
        }
    }

    public bool ArrowVisible
    {
        get => fakeArrow.activeSelf;
        set => fakeArrow.SetActive(value);
    }

    private Material material;

    private void Start()
    {
        var renderer = GetComponent<Renderer>();
        // Render.material is unique to this object
        material = renderer.material;

        totalFrames = material.GetFloat(TotalFramesVar);
    }

    private void UpdateAnimation()
    {
        float modDrawLevel = Mathf.Lerp(animOffset, 1.0f, drawLevel);
        float currentFrame = Mathf.Clamp(modDrawLevel * totalFrames, 0, totalFrames - 1);
        material.SetFloat(CurrentFrameVar, drawLevel * currentFrame);

        connectTop.LerpPosition(DrawLevel);
        connectBottom.LerpPosition(DrawLevel);
        nock.LerpPosition(DrawLevel);
    }
}
