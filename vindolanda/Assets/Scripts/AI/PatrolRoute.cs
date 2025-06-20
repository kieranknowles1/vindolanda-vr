using System;
using System.Collections.Generic;
using UnityEngine;

// Need a GuidComponent to be referenced in saves
[RequireComponent(typeof(GuidComponent))]
public class PatrolRoute : MonoBehaviour
{
    public enum Type
    {
        BackForth,
        Loop,
        OneShot
    }

    public enum Direction
    {
        Forward,
        Backward,
    }

    [Serializable]
    public struct PointInfo
    {
        public int index;
        public Direction direction;
        // Has this route looped back to the start?
        public bool looped;
        // Is the route complete?
        public bool done;
    }

    public Type type = Type.Loop;

    public List<Transform> points;

    public PointInfo GetNextPoint(PointInfo previous)
    {
        int offset = previous.direction == Direction.Forward ? 1 : -1;
        int idx = previous.index + offset;
        if (idx < 0 || idx >= points.Count)
        {
            return type switch
            {
                Type.Loop => new()
                {
                    direction = previous.direction,
                    index = previous.direction == Direction.Forward ? 0 : points.Count - 1,
                    looped = true
                },
                Type.BackForth => new()
                {
                    direction = previous.direction == Direction.Forward ? Direction.Backward : Direction.Forward,
                    index = previous.index - offset,
                    looped = true
                },
                Type.OneShot => new() { done = true },
                _ => throw new System.Exception()
            };
        }

        return new()
        {
            direction = previous.direction,
            index = idx
        };
    }

    private void OnDrawGizmos()
    {
        if (points == null) return;

        var previous = new PointInfo() { direction = Direction.Forward, index = 0 };
        do
        {
            var current = GetNextPoint(previous);
            if (current.done) break;

            Transform currentT = points[current.index];
            Transform prevT = points[previous.index];
            Gizmos.color = Color.green;
            Gizmos.DrawLine(currentT.position, prevT.position);
            var aToB = (currentT.position - prevT.position).normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(prevT.position + aToB, Vector3.one);

            previous = current;
        } while (!previous.looped);
    }
}
