using UnityEngine;

public class ChaseEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHASE ENEMY STATE";
    #endregion
    public ChaseEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        enemyAI.currentState = EnemyStatesEnum.Chase;
        enemyAI.attackRangeDetector.attackCollider.enabled = false;

        agent.speed = enemyData.ChaseSpeed;
        agent.isStopped = false;
        agent.stoppingDistance = enemyData.AttackRange;
    }

    public override void Update(float distToPlayer)
    {
        animator.SetFloat(_animSpeedParameter, 1, 0.2f, Time.deltaTime);
        agent.SetDestination(enemyAI.player.position);


        if (distToPlayer > enemyData.ChaseRange) fsm.SwitchState(fsm.patrolState);
        else if (distToPlayer <= enemyData.AttackRange) fsm.SwitchState(fsm.attackState);
    }

    public override void Exit()
    {
    }

}