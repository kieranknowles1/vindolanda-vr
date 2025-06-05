using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

internal static class NavUtil
{
    /// <summary>
    /// Select a random point on the navmesh within <paramref name="range"/> units of <paramref name="position"/>,
    /// 
    /// Outputs to <paramref name="target"/>
    /// </summary>
    /// <param name="maxAttempts">Number of attempts before giving up</param>
    /// <returns>True on success, false on failure. Target is not meaningful on failure.</returns>
    internal static bool RandomNavmeshPosition(Vector3 position, float range, out Vector3 target, int maxAttempts = 5)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position + randomDirection, out hit, 5.0f, NavMesh.AllAreas))
            {
                target = hit.position;
                return true;
            }
        }

        target = Vector3.zero;
        return false;
    }

    internal static bool ReachedDestination(NavMeshAgent agent, float threshold = 0.2f)
    {
        return agent.remainingDistance < threshold;
    }
}