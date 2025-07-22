using UnityEngine.XR.Hands;

public static class EnumExtensions
{
    public static XRHandFingerID GetFinger(this XRHandJointID joint) => joint switch
    {
        XRHandJointID.ThumbMetacarpal or
        XRHandJointID.ThumbProximal or
        XRHandJointID.ThumbDistal or
        XRHandJointID.ThumbTip => XRHandFingerID.Thumb,

        XRHandJointID.IndexMetacarpal or
        XRHandJointID.IndexProximal or
        XRHandJointID.IndexIntermediate or
        XRHandJointID.IndexDistal or
        XRHandJointID.IndexTip => XRHandFingerID.Index,

        XRHandJointID.MiddleMetacarpal or
        XRHandJointID.MiddleProximal or
        XRHandJointID.MiddleIntermediate or
        XRHandJointID.MiddleDistal or
        XRHandJointID.MiddleTip => XRHandFingerID.Middle,

        XRHandJointID.RingMetacarpal or
        XRHandJointID.RingProximal or
        XRHandJointID.RingIntermediate or
        XRHandJointID.RingDistal or
        XRHandJointID.RingTip => XRHandFingerID.Ring,

        XRHandJointID.LittleMetacarpal or
        XRHandJointID.LittleProximal or
        XRHandJointID.LittleIntermediate or
        XRHandJointID.LittleDistal or
        XRHandJointID.LittleTip => XRHandFingerID.Little,

        _ => throw new UnreachableException()
    };

    public static XRHandJointID GetProximalJoint(this XRHandFingerID finger) => finger switch
    {
        XRHandFingerID.Thumb => XRHandJointID.ThumbProximal,
        XRHandFingerID.Index => XRHandJointID.IndexProximal,
        XRHandFingerID.Middle => XRHandJointID.MiddleProximal,
        XRHandFingerID.Ring => XRHandJointID.RingProximal,
        XRHandFingerID.Little => XRHandJointID.LittleProximal,
        _ => throw new UnreachableException()
    };

}
