using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class CustomColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var collider = (Collider)target;

        bool isTeleportable = collider.GetComponent<TeleportationArea>() != null;

        if (!isTeleportable && GUILayout.Button("Make Teleport Target"))
        {
            Undo.RecordObject(collider, "Make teleportable");
            var area = collider.AddComponent<TeleportationArea>();
            area.colliders.Add(collider);
            area.interactionLayers = InteractionLayerMask.GetMask("Teleport");
        }
    }
}

// Workaround since we can't attach multiple CustomEditor attributes
[CustomEditor(typeof(TerrainCollider))]
public class CustomTerrainColliderEditor : CustomColliderEditor { }

[CustomEditor(typeof(BoxCollider))]
public class CustomBoxColliderEditor : CustomColliderEditor { }