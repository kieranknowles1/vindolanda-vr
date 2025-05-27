using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ActorController : MonoBehaviour
{
    public enum ActorState
    {
        Init, // Nothing happened yet
        Idle, // Waiting between actions
        Active, // Has an active action
    };

    private NavMeshAgent agent;
    private Vector3 homePosition;
    public ActorState State { get; private set; } = ActorState.Init;

    public float WanderRadius = 10;
    public float SleepMin = 2.0f;
    public float SleepMax = 5.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }


    /// <summary>
    /// Attempt to travel to a random point within <see cref="WanderRadius"/> units of
    /// the actor's starting position.
    /// </summary>
    bool PickWander()
    {
        Vector2 randomDirection = Random.insideUnitCircle * WanderRadius;
        Vector3 target = homePosition + new Vector3(randomDirection.x, 0, randomDirection.y);
        if (!NavMesh.SamplePosition(target, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            return false;
        }

        agent.SetDestination(hit.position);
        return true;
    }

    bool HasReachedDestination()
    {
        if (State == ActorState.Idle)
            return false; // We're not doing anything currently
        if (agent.pathPending)
            return false; // We're still calculating a path
        if (agent.remainingDistance > agent.stoppingDistance)
            return false; // Haven't reached the target yet
        // Return true if we've stopped
        return !agent.hasPath || agent.velocity.sqrMagnitude == 0;
    }

    IEnumerator SleepAndPickTarget()
    {
        State = ActorState.Idle;
        float duration = Random.Range(SleepMin, SleepMax);
        yield return new WaitForSeconds(duration);
        PickWander();
        State = ActorState.Active;
    }

    // Update is called once per frame
    void Update()
    {
        if (State == ActorState.Init || HasReachedDestination())
        {
            StartCoroutine(SleepAndPickTarget());
        }
    }
}
