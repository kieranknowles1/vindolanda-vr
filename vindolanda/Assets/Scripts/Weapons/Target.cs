using System;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IHitTarget
{
    [Serializable]
    public struct HitArea
    {
        public float radius;
        public int value;
    }

    // It's not shoddy texturing, it's realistic compensation for the lack of modern techniques
    public Vector3 offset;
    public List<HitArea> areas;

    public Vector3 Center => transform.position + (transform.rotation * offset);

    public void OnHit(IWeapon weapon)
    {
        // TODO: Award points based on proximity to center
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        
        foreach (var area in areas)
        {
            GizmoUtil.DrawCircle(Center, transform.rotation, area.radius);
        }
    }
}
