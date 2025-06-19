using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    [Tooltip("Event called with score of the hit")]
    public UnityEvent<int> OnArrowHit;

    public Vector3 Center => transform.position + (transform.rotation * offset);

    int CalculateScore(Arrow arrow)
    {
        float distance = (arrow.tip.position - Center).magnitude;
        return areas.Where(a => a.radius <= distance).Cast<HitArea?>().FirstOrDefault()?.value ?? 1;
    }

    public void OnHit(IWeapon weapon)
    {
        if (weapon is not Arrow arrow) return;

        OnArrowHit?.Invoke(CalculateScore(arrow));
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
