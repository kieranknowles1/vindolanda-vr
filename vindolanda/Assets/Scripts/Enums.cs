using System;
using UnityEngine;

public static class Enums
{
    [Serializable]
    public enum Axis
    {
        X, Y, Z
    }

    public static Vector3 Forward(this Axis axis) => axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => throw new UnreachableException()
        };
}