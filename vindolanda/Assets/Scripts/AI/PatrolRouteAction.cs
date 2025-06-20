using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolRouteAction", story: "[Agent] patrols along [Route]", category: "Action/Navigation", id: "fb78bd599f12fa6f67240335f31fa20a")]
public partial class PatrolRouteAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<PatrolRoute> Route;
    [SerializeReference] public BlackboardVariable<float> Speed = new(3f);
    [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
    [Tooltip("Should patrol restart from the latest point?")]
    [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);
    [SerializeReference] public BlackboardVariable<int> StartingWaypoint = new(0);

    private NavMeshAgent m_NavMeshAgent;
    [CreateProperty] private Vector3 m_CurrentTarget;
    [CreateProperty] private float m_OriginalStoppingDistance = -1f;
    [CreateProperty] private float m_OriginalSpeed = -1f;
    [CreateProperty] private float m_WaypointWaitTimer;
    [CreateProperty] private PatrolRoute.PointInfo m_CurrentPatrolPoint;
    [CreateProperty] private bool m_Waiting;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent assigned.");
            return Status.Failure;
        }

        if (Route.Value == null || Route.Value.points.Count == 0)
        {
            LogFailure("No waypoints to patrol assigned.");
            return Status.Failure;
        }

        m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent == null)
        {
            LogFailure("No NavMeshAgent on agent");
            return Status.Failure;
        }

        Initialize();

        m_Waiting = false;
        m_WaypointWaitTimer = 0.0f;
        m_CurrentPatrolPoint = new() { direction = PatrolRoute.Direction.Forward, index = StartingWaypoint.Value };
        MoveToNextWaypoint();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (m_CurrentPatrolPoint.done) return Status.Success;

        if (m_Waiting)
        {
            if (m_WaypointWaitTimer > 0.0f)
            {
                m_WaypointWaitTimer -= Time.deltaTime;
            }
            else
            {
                m_WaypointWaitTimer = 0f;
                m_Waiting = false;
                MoveToNextWaypoint();
            }
        }
        else
        {
            float distance = m_NavMeshAgent.remainingDistance;
            bool destinationReached = distance <= DistanceThreshold;

            // Check if we've reached the waypoint (ensuring NavMeshAgent has completed path calculation if available)
            if (destinationReached && (m_NavMeshAgent == null || !m_NavMeshAgent.pathPending))
            {
                m_WaypointWaitTimer = WaypointWaitTime.Value;
                m_Waiting = true;

                return Status.Running;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
        }
        m_NavMeshAgent.speed = m_OriginalSpeed;
        m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;
    }

    protected override void OnDeserialize()
    {
        // If using a navigation mesh, we need to reset default value before Initialize.
        m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_OriginalSpeed >= 0f)
            m_NavMeshAgent.speed = m_OriginalSpeed;
        if (m_OriginalStoppingDistance >= 0f)
            m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;

        m_NavMeshAgent.Warp(Agent.Value.transform.position);

        bool preserve = PreserveLatestPatrolPoint.Value;
        PreserveLatestPatrolPoint.Value = true;
        // During deserialization, consider PreserveLatestPatrolPoint always true.
        Initialize();
        PreserveLatestPatrolPoint.Value = preserve;
    }

    private void Initialize()
    {
        if (m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
        }

        m_OriginalSpeed = m_NavMeshAgent.speed;
        m_NavMeshAgent.speed = Speed.Value;
        m_OriginalStoppingDistance = m_NavMeshAgent.stoppingDistance;
        m_NavMeshAgent.stoppingDistance = DistanceThreshold;

        // MoveToNextWaypoint will increment index
        int off = m_CurrentPatrolPoint.direction == PatrolRoute.Direction.Forward ? 1 : -1;
        m_CurrentPatrolPoint.index -= off;
    }

    private void MoveToNextWaypoint()
    {
        m_CurrentPatrolPoint = Route.Value.GetNextPoint(m_CurrentPatrolPoint);
        if (m_CurrentPatrolPoint.done) return;

        m_CurrentTarget = Route.Value.points[m_CurrentPatrolPoint.index].transform.position;
        m_NavMeshAgent.SetDestination(m_CurrentTarget);
    }
}