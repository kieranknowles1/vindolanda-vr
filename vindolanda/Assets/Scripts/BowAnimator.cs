using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BowAnimator : MonoBehaviour
{
    static readonly int CurrentFrameVar = Shader.PropertyToID("_frame");
    static readonly int TotalFramesVar = Shader.PropertyToID("_frames");

    [SerializeField, Range(0, 1)] float drawLevel;
    float totalFrames;

    public float DrawLevel
    {
        get => drawLevel;
        set {
            drawLevel = value;
            UpdateAnimation();
        }
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
        float currentFrame = Mathf.Clamp(drawLevel * totalFrames, 0, totalFrames - 1);
        material.SetFloat(CurrentFrameVar, drawLevel * currentFrame);
    }

    // TODO: The property handles animations. This is only useful for the inspector
    private void Update()
    {
        UpdateAnimation();        
    }
}
