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
            var area = Undo.AddComponent<TeleportationArea>(collider.gameObject);
            area.colliders.Add(collider);
            area.interactionLayers = InteractionLayerMask.GetMask("Teleport");
        }
    }
}

// TODO: Can I avoid replacing the original completely?
//// Workaround since we can't attach multiple CustomEditor attributes
//[CustomEditor(typeof(TerrainCollider))]
//public class CustomTerrainColliderEditor : CustomColliderEditor { }

//[CustomEditor(typeof(BoxCollider))]
//public class CustomBoxColliderEditor : CustomColliderEditor { }

//[CustomEditor(typeof(MeshCollider))]
//public class MeshColliderEditor : CustomColliderEditor { }