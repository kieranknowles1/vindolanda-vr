using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ActorAnimator : MonoBehaviour
{
    const string SpeedVariable = "SpeedMagnitude";

    Animator animator;
    Vector3 previousPosition;

    bool halted;
    public bool Halted
    {
        get => halted;
        set {
            halted = value;
            animator.SetFloat(SpeedVariable, 0);
            previousPosition = transform.position;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if (halted) return;
        float speed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;

        animator.SetFloat(SpeedVariable, speed);
    }

    /// <summary>
    /// Animation event callback
    /// </summary>
    public void OnFootDown(string foot)
    {
        // If a tree falls and no one is around to hear it, does it make a sound?
        // Don't do anything if no one can here us walk
        // TODO: Only play if nearby

        var bone = animator.GetBoneTransform(foot == "Left" ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
        if (!Physics.Raycast(new Ray(bone.position + (0.25f * Vector3.up), Vector3.down), out var hit, 1.0f)) return;

        var mat = hit.collider.sharedMaterial;
        var footsteps = MaterialData.GetExtraData(mat).footsteps;
        if (footsteps == null) return;

        var clip = footsteps.walk[Random.Range(0, footsteps.walk.Count)];
        // TODO: Adjust volume
        AudioSource.PlayClipAtPoint(clip, bone.position, 1.0f);
    }
}
