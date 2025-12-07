using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "PATROL ENEMY STATE";
    #endregion

    private Transform[] waypoints => _enemyAI.GetCurrentWaypoints();
    public int currentWaypointIndex = 0;
    private bool isWaitingAtWaypoint = false;
    private float _waitTimer = 0f;

    public PatrolEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.Patrol;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Reset confirmation flag when returning to patrol (after losing player)
        _enemyAI.HasConfirmedPlayer = false;
        
        // Reset noise chase flag: quando torno in patrol, considero chiusa qualsiasi noise-chase
        _enemyAI.IsNoiseChaseActive = false;

        // Ensure we have valid waypoints (from current patrol area or legacy waypoints)
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.W("PatrolEnemyState: No waypoints available. Cannot patrol.", _LOG_COLOR, _LOG_TAG);
            return;
        }

        _agent.isStopped = false;
        _agent.stoppingDistance = 0f;

        if (!isWaitingAtWaypoint)
            GotoNextWaypoint();
    }
    public override void Update(float attraction)
    {
        _agent.speed = _enemyData.PatrolSpeed * _enemyData.ChaseSpeed;

        float targetSpeed = isWaitingAtWaypoint ? 0f : _enemyData.PatrolSpeed;
        _animator.SetFloat(_animSpeedParameter, targetSpeed, 0.4f, Time.deltaTime);

        // Check for confusing sound source (before other checks)
        if (_enemyAI.ShouldBeDistractedByConfusingSound())
        {
            _fsm.SwitchState(_fsm.gettingConfusedState);
            return;
        }

        // Note: Global triggers in EnemyFSM handle:
        // - A >= 1.0 → MandatoryChaseState
        // - d <= 10m → ChaseDistanceState
        // So PatrolState just continues patrolling if conditions are met

        if (isWaitingAtWaypoint)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                isWaitingAtWaypoint = false;
                GotoNextWaypoint();
            }
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _enemyData.WaypointArrivalThreshold)
        {
            StartWaitAtWaypoint();
        }
    }

    public override void Exit()
    {
        isWaitingAtWaypoint = false;
        _waitTimer = 0f;
        _agent.isStopped = false;
    }


    private void StartWaitAtWaypoint()
    {
        isWaitingAtWaypoint = true;
        _waitTimer = _enemyData.WaypointPauseDuration;

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    void GotoNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.W("GotoNextWaypoint: No waypoints available.", _LOG_COLOR, _LOG_TAG);
            return;
        }
        
        isWaitingAtWaypoint = false;

        _agent.isStopped = false;
        _agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}