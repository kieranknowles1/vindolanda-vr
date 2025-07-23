using UnityEngine;

namespace Vindolanda.Animation
{

    [RequireComponent(typeof(Animator))]
    public class ActorAnimator : MonoBehaviour
    {
        const string SpeedVariable = "SpeedMagnitude";
        const float RunFootstepThreshold = 2.0f;

        Animator animator;
        Vector3 previousPosition;

        bool halted;
        public bool Halted
        {
            get => halted;
            set
            {
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
        /// Select a footstep sound based on this actor's speed
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        AudioClip SelectFootstepSound(FootstepData steps)
        {
            float speed = animator.GetFloat(SpeedVariable);
            var clips = speed switch
            {
                < RunFootstepThreshold => steps.walk,
                _ => steps.run,
            };
            return clips != null && clips.Count > 0 ? clips[Random.Range(0, clips.Count)] : null;
        }

        /// <summary>
        /// Animation event callback
        /// 
        /// NOTE: Only the walking animation should send this event, it will fire even if its blend ratio
        /// is zero (running), and avoids footsteps being duplicated by each blended animation. It is assumed
        /// that all walk/run animations are the same duration and place their feet down at the same points.
        /// </summary>
        public void OnFootDown(string foot)
        {
            // If a tree falls and no one is around to hear it, does it make a sound?
            // Don't do anything if no one can here us walk
            var distance = (transform.position - GameConstants.Instance.Player.transform.position).magnitude;
            if (distance > 20) return;

            var bone = animator.GetBoneTransform(foot == "Left" ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
            if (!Physics.Raycast(new Ray(bone.position + (0.25f * Vector3.up), Vector3.down), out var hit, 1.0f)) return;

            var mat = hit.collider.sharedMaterial;
            var footsteps = MaterialData.GetExtraData(mat).footsteps;
            if (footsteps == null) return;

            var clip = SelectFootstepSound(footsteps);
            if (clip == null) return;
            // TODO: Adjust volume
            AudioSource.PlayClipAtPoint(clip, bone.position, 1.0f);
        }
    }
}