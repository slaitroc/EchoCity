using UnityEngine;

[System.Serializable]
public class ChaseEnemyState : EnemyState
{
    public ChaseEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm)
    {
    }

    public override void Enter()
    {
        enemyAI.currentState = EnemyStatesEnum.Chase;
        agent.speed = enemyData.ChaseSpeed;
        agent.isStopped = false;
        agent.stoppingDistance = enemyData.AttackRange;
    }
    public override void Update(float distToPlayer)
    {
        animator.SetFloat(_speedParameter, 1, 0.2f, Time.deltaTime);
        agent.SetDestination(enemyAI.player.position);


        if (distToPlayer > enemyData.ChaseRange) fsm.SwitchState(fsm.patrolState);
        else if (distToPlayer <= enemyData.AttackRange) fsm.SwitchState(fsm.attackState);
    }

    public override void Exit()
    {
    }

}