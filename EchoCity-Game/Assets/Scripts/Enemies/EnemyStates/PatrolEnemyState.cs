using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "PATROL ENEMY STATE";
    #endregion

    private Transform[] waypoints => _enemyAI.waypoints;
    public int currentWaypointIndex = 0;
    private bool isWaitingAtWaypoint = false;
    private float _waitTimer = 0f;

    public PatrolEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.Patrol;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

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

        // Check attraction - if threshold is exceeded, start chasing
        // States only react to attraction (already calculated by EnemyAI)
        if (attraction >= _enemyData.NoiseThreshold)
        {
            // Switch to chase state (will chase towards sound position, not player)
            _fsm.SwitchState(_fsm.chaseState);
            return;
        }

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