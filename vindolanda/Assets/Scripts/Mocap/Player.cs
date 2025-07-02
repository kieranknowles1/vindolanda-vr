using UnityEngine;

namespace Vindolanda.Mocap
{
    public class Player : MonoBehaviour
    {
        public Clip clip;
        public bool repeat;

        public GestureRenderer leftHand;
        public GestureRenderer rightHand;
        public Transform head;

        [Header("State")]
        public bool playing;
        public int frame;
        float time;

        Clip.Keyframe CurrentFrame => clip.keyframes[frame];

        private void Update()
        {
            if (!playing) return;

            time += Time.deltaTime;
            while (CurrentFrame.time < time)
            {
                frame++;

                if (frame >= clip.keyframes.Count)
                {
                    frame = 0;
                    time = 0;
                    break;
                }
            }

            static void UpdateHand(GestureRenderer hand, Clip.HandState state)
            {
                if (hand == null) return;
                hand.transform.SetLocalPositionAndRotation(state.transform.position, state.transform.rotation);
                hand.SetShapeInstant(state);
            }
            UpdateHand(leftHand, CurrentFrame.leftHand);
            UpdateHand(rightHand, CurrentFrame.rightHand);
            if (head)
                head.SetLocalPositionAndRotation(CurrentFrame.head.position, CurrentFrame.head.rotation);
        }
    }
}