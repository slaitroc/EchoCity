using UnityEngine;

public class ChaseEnemyState : EnemyState
{
#pragma warning disable CS0414
    #region Constants
    protected new string _LOG_TAG = "CHASE ENEMY STATE";
    #endregion
#pragma warning restore CS0414
    public ChaseEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.Chase;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
    }

    public override void Update(float distToPlayer)
    {
        _animator.SetFloat(_animSpeedParameter, 1, 0.2f, Time.deltaTime);
        _agent.SetDestination(_enemyAI.player.position);


        if (distToPlayer > _enemyData.LoseRange) _fsm.SwitchState(_fsm.patrolState);
        else if (distToPlayer <= _enemyData.AttackRange) _fsm.SwitchState(_fsm.attackState);
    }

    public override void Exit()
    {
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}