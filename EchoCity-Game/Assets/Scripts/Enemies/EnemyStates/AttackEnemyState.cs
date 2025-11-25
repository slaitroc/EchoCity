using System.Collections;
using UnityEngine;

public class AttackEnemyState : EnemyState
{
#pragma warning disable CS0414
    #region Constants
    protected new string _LOG_TAG = "ATTACK ENEMY STATE";
    #endregion
#pragma warning restore CS0414

    private bool _attackEnded;
    private float _coolDownTimer;

    public AttackEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _coolDownTimer = 0f;
        _attackEnded = false;
        _enemyAI.CurrentState = EnemyStatesEnum.Attack;
        _enemyAI.StartCoroutine(AttackRoutine());
    }
    public override void Update(float distToPlayer)
    {
        _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);

        if (_attackEnded)
        {
            Vector3 dir = _enemyAI.player.position - _enemyAI.transform.position;
            dir.y = 0f; // Keep only horizontal direction
            Quaternion targetRot = Quaternion.LookRotation(dir);
            _enemyAI.transform.rotation = Quaternion.Slerp(
                _enemyAI.transform.rotation,
                targetRot,
                _enemyData.CoolDownRotationSpeed * Time.deltaTime
            );
            _coolDownTimer += Time.deltaTime;
            if (_coolDownTimer >= _enemyData.AttackCoolDown)
                _fsm.SwitchState(_fsm.chaseState);
        }
    }


    public override void Exit() { }

    IEnumerator AttackRoutine()
    {
        _animator.SetBool(_animIsAttacking, true);
        _agent.isStopped = true;

        yield return new WaitForSeconds(_enemyData.AttackDamageDelay);
        _enemyAI.attackRangeDetector.attackCollider.enabled = true;

        yield return new WaitForSeconds(_enemyData.AttackDamageWindowTime);
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        yield return new WaitForSeconds(_enemyData.AttackDuration - _enemyData.AttackDamageDelay - _enemyData.AttackDamageWindowTime);
        _animator.SetBool(_animIsAttacking, false);
        _attackEnded = true;

    }

    //Called by AttackRangeDetector(->EnemyAI->this) when player is hit
    public override bool OnPlayerHit()
    {
        // we can add some reaction to player hit if needed
        Log.D("Player hit by enemy attack!", _LOG_COLOR, _LOG_TAG);
        return true;
    }

}