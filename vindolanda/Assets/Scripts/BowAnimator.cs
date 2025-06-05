using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BowAnimator : MonoBehaviour
{
    static readonly int CurrentFrameVar = Shader.PropertyToID("_frame");
    static readonly int TotalFramesVar = Shader.PropertyToID("_frames");

    [SerializeField, Range(0, 1)] float drawLevel;
    float totalFrames;

    float animOffset = 0.33f;

    [SerializeField] Transform connectTopStart;
    [SerializeField] Transform connectTopEnd;

    [SerializeField] Transform connectBottomStart;
    [SerializeField] Transform connectBottomEnd;

    [SerializeField] Transform nockStart;
    [SerializeField] Transform nockEnd;

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
        float modDrawLevel = Mathf.Lerp(animOffset, 1.0f, drawLevel);
        float currentFrame = Mathf.Clamp(modDrawLevel * totalFrames, 0, totalFrames - 1);
        material.SetFloat(CurrentFrameVar, drawLevel * currentFrame);
    }

    // TODO: The property handles animations. This is only useful for the inspector
    private void Update()
    {
        UpdateAnimation();        
    }

    // TODO: Proper rendering for bowstring
    private void DrawBowstring(Transform connectStart, Transform connectEnd, Vector3 nockPosition)
    {
        Gizmos.color = Color.black;
        var connectPosition = Vector3.Lerp(connectStart.position, connectEnd.position, drawLevel);
        Gizmos.DrawLine(nockPosition, connectPosition);
    }

    private void OnDrawGizmos()
    {
        var nockPosition = Vector3.Lerp(nockStart.position, nockEnd.position, drawLevel);
        DrawBowstring(connectTopStart, connectTopEnd, nockPosition);
        DrawBowstring(connectBottomStart, connectBottomEnd, nockPosition);
    }
}
