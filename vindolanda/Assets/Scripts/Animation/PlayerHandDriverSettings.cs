using UnityEngine;

namespace Vindolanda.Animation
{
    // These options are defined on a scriptable object so that we can tweak them in play mode
    // and persist changes into edit mode. Milimeter precision is needed for most of these
    [CreateAssetMenu(menuName = "Settings/Hand Driver Settings")]
    public class PlayerHandDriverSettings : ScriptableObject
    {
        public Quaternion RotateAroundOrigin = Quaternion.identity;

        // XR hands use a different coordinate system to the default skeleton :(
        // Convert between them here
        public Quaternion RotationOffset = Quaternion.Euler(90.0f, 0, 0);
        // Don't deform fingers if the arm is stretched too far. Most noticable for players with
        // long arms, or if leaning back
        public float PositionToleranceSquared = 0.01f * 0.01f;

        // Lerp between these offsets to further reduce deformation
        public Vector3 PositionOffsetBase = new(0.0135f, -0.0115f, 0);
        // Yes, that's a 3mm offset. Even that level of error is noticable when applied to the knuckles
        public Vector3 PositionOffsetDelta = new(-0.005f, 0, 0.003f);
        public Vector3 PositionOffsetMax => PositionOffsetBase + PositionOffsetDelta;

        public Quaternion SelfRotationOffset = Quaternion.identity;
        public Vector3 SelfPositionOffset = Vector3.zero;
    }
}