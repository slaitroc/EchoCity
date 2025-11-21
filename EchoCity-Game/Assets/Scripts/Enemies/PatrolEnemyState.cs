using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolEnemyState : EnemyState
{
    private Transform[] waypoints => enemyAI.waypoints;
    public int currentWaypointIndex = 0;
    private bool isWaitingAtWaypoint = false;
    private float _waitTimer = 0f;

    public PatrolEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        enemyAI.currentState = EnemyStatesEnum.Patrol;
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        if (!isWaitingAtWaypoint)
            GotoNextWaypoint();
    }
    public override void Update(float distToPlayer)
    {

        agent.speed = enemyData.PatrolSpeed * enemyData.ChaseSpeed;

        float targetSpeed = isWaitingAtWaypoint ? 0f : enemyData.PatrolSpeed;
        animator.SetFloat(_speedParameter, targetSpeed, 0.4f, Time.deltaTime);

        if (distToPlayer < enemyData.ChaseRange) fsm.SwitchState(fsm.chaseState);

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

        if (!agent.pathPending && agent.remainingDistance <= enemyData.WaypointArrivalThreshold)
        {
            StartWaitAtWaypoint();
        }

    }

    public override void Exit()
    {
        isWaitingAtWaypoint = false;
        _waitTimer = 0f;
        agent.isStopped = false;
    }


    private void StartWaitAtWaypoint()
    {
        isWaitingAtWaypoint = true;
        _waitTimer = enemyData.WaypointPauseDuration;

        agent.isStopped = true;
        agent.ResetPath();
    }

    void GotoNextWaypoint()
    {
        isWaitingAtWaypoint = false;

        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}