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
    Mesh dummyMesh;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.gameObjects.Any(o => o.transform == target || o == gameObject))
            return;

        Gizmos.color = Color.purple;
        Handles.Label(target.transform.position + (Vector3.up * 2), "Teleport Target");
        Gizmos.DrawWireMesh(dummyMesh, target.transform.position, target.transform.rotation);
    }

#endif
    protected override void Execute(PlayerController player)
    {
        player.transform.position = target.position;
        player.transform.rotation = target.rotation;
    }
}
