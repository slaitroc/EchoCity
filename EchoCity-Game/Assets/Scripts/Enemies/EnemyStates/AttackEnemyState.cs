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
        _enemyAI.CurrentState = EnemyStatesEnum.Attack;
        _enemyAI.StartCoroutine(AttackRoutine());
    }
    public override void Update(float attraction)
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
            
            // Check if player is still in chase range - if so, skip cooldown and resume chase immediately
            float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
            
            // For immediate chase resumption, use D_exit (15m) when HasConfirmedPlayer is true
            bool playerStillInChaseRange = _enemyAI.HasConfirmedPlayer 
                ? distToPlayer <= _enemyData.D_exit 
                : distToPlayer <= _enemyData.D_enter;
            
            // If player is still close, skip cooldown and resume chase immediately
            if (playerStillInChaseRange && _coolDownTimer >= 0.1f) // Small delay to allow rotation
            {
                bool noiseChase = _enemyAI.IsNoiseChaseActive;
                
                if (noiseChase)
                {
                    _fsm.SwitchState(_fsm.chaseState);
                }
                else
                {
                    _fsm.SwitchState(_fsm.chaseDistanceState);
                }
                return;
            }
            
            // Otherwise, wait for full cooldown
            if (_coolDownTimer >= _enemyData.AttackCoolDown)
            {
                bool noiseChase = _enemyAI.IsNoiseChaseActive;

                if (noiseChase)
                {
                    // CASO 1: inseguimento nato da rumore (threshold superata oppure da CheckSound)
                    // → ha senso usare attraction / threshold e poter tornare a CheckSound
                    if (attraction >= _enemyData.NoiseThreshold || distToPlayer <= _enemyData.D_enter)
                    {
                        // ancora molto attratto o ancora vicino → continua a inseguire normalmente
                        _fsm.SwitchState(_fsm.chaseState);
                    }
                    else if (attraction < _enemyData.NoiseLoseThreshold)
                    {
                        // attrazione bassa ma l'ultimo suono potrebbe essere ancora rilevante → CheckSound
                        _fsm.SwitchState(_fsm.checkSoundState);
                    }
                    else
                    {
                        // caso intermedio: attraction tra LoseThreshold e Threshold → continua a cercare
                        _fsm.SwitchState(_fsm.chaseState);
                    }
                }
                else
                {
                    // CASO 2: inseguimento nato SOLO da distanza (ChaseDistance/Attack senza threshold > 1)
                    // → NON ha senso usare CheckSound, ci basiamo solo sulla distanza.
                    if (distToPlayer <= _enemyData.D_exit)
                    {
                        // Il player è ancora entro D_exit (15m) → continua a inseguire in ChaseDistanceState
                        _fsm.SwitchState(_fsm.chaseDistanceState);
                    }
                    else
                    {
                        // Il player è oltre D_exit (15m) → LostTargetState
                        _fsm.SwitchState(_fsm.lostTargetState);
                    }
                }
            }
        }
    }


    public override void Exit()
    {
        // Solo cleanup, NIENTE SwitchState qui dentro
        _animator.SetBool(_animIsAttacking, false);
        _attackEnded = false;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;
    }


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

    public override bool OnPlayerHit()
    {
        Log.D($"{_enemyAI.name} hit the player!", _LOG_COLOR, _LOG_TAG);
        return true;
    }
}