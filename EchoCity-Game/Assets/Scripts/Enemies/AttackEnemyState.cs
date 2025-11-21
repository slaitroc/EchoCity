using System.Collections;
using UnityEngine;

[System.Serializable]
public class AttackEnemyState : EnemyState
{
    private Coroutine attackCoroutine;
    private float _lastAttackTime;

    public AttackEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm)
    {
    }

    public override void Enter()
    {
        enemyAI.currentState = EnemyStatesEnum.Attack;

        animator.SetTrigger(_attackTrigger);

        _lastAttackTime = Time.time;
        attackCoroutine = enemyAI.StartCoroutine(AttackRoutine());


    }
    public override void Update(float distToPlayer)
    {
        animator.SetFloat(_speedParameter, 0f, 0.2f, Time.deltaTime);
    }


    public override void Exit()
    {
    }

    IEnumerator AttackRoutine()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        yield return new WaitForSeconds(enemyData.AttackDuration);

        agent.isStopped = false;
        attackCoroutine = null;

        fsm.SwitchState(fsm.chaseState);
    }
}