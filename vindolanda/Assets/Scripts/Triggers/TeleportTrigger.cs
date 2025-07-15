using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TeleportTrigger : TriggerBase
{
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform followerTarget;

    [SerializeField]
    Mesh dummyMesh;

#if UNITY_EDITOR
    void DrawGizmo(Color color, Transform obj, string label)
    {
        Gizmos.color = color;
        Handles.Label(obj.position + (Vector3.up * 2), label);
        Gizmos.DrawWireMesh(dummyMesh, obj.position, obj.rotation);
    }

    private void OnDrawGizmos()
    {
        if (!Selection.gameObjects.Any(o => o.transform == target || o == gameObject || o.transform == followerTarget))
            return;

        DrawGizmo(Color.purple, target, "Teleport Target");
        DrawGizmo(Color.mediumPurple, followerTarget, "Follower Target");
    }

#endif
    protected override void Execute(PlayerController player)
    {
        player.transform.Teleport(target, alignRotation: true);
        if (player.TryGetComponent<FollowerTracker>(out var followers))
        {
            foreach (var follower in followers.followers)
            {
                follower.transform.Teleport(followerTarget, alignRotation: true);
            }
        }
    }
}
