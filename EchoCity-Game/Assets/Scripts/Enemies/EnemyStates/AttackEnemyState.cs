using System.Collections;
using UnityEngine;

public class AttackEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "ATTACK ENEMY STATE";
    #endregion

    private bool _attackEnded;
    private float _coolDownTimer;

    public AttackEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _coolDownTimer = 0f;
        _attackEnded = false;
        enemyAI.currentState = EnemyStatesEnum.Attack;
        enemyAI.StartCoroutine(AttackRoutine());
    }
    public override void Update(float distToPlayer)
    {
        animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);

        if (_attackEnded)
        {
            Vector3 dir = enemyAI.player.position - enemyAI.transform.position;
            dir.y = 0f; // Keep only horizontal direction
            Quaternion targetRot = Quaternion.LookRotation(dir);
            enemyAI.transform.rotation = Quaternion.Slerp(
                enemyAI.transform.rotation,
                targetRot,
                enemyData.CoolDownRotationSpeed * Time.deltaTime
            );
            _coolDownTimer += Time.deltaTime;
            if (_coolDownTimer >= enemyData.AttackCoolDown)
                fsm.SwitchState(fsm.chaseState);
        }
    }


    public override void Exit() { }

    IEnumerator AttackRoutine()
    {
        animator.SetBool(_animIsAttacking, true);
        agent.isStopped = true;

        yield return new WaitForSeconds(enemyData.AttackDamageDelay);
        enemyAI.attackRangeDetector.attackCollider.enabled = true;

        yield return new WaitForSeconds(enemyData.AttackDamageWindowTime);
        enemyAI.attackRangeDetector.attackCollider.enabled = false;

        yield return new WaitForSeconds(enemyData.AttackDuration - enemyData.AttackDamageDelay - enemyData.AttackDamageWindowTime);
        animator.SetBool(_animIsAttacking, false);
        _attackEnded = true;

    }

    //Called by AttackRangeDetector script when player is hit
    public void PlayerHitHandler(EnemyAI enemy)
    {
        Log.D($"{enemyAI.name} hit the player!", _LOG_COLOR, _LOG_TAG);
    }

}