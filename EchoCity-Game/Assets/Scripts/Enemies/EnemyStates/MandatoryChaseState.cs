using UnityEngine;

/// <summary>
/// Mandatory chase state: 3 seconds of guaranteed chase towards player.
/// Triggered globally when attraction >= 1.0 (A_enter).
/// After 3 seconds, transitions to ChaseState or CheckSoundState based on conditions.
/// </summary>
public class MandatoryChaseState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "MANDATORY CHASE STATE";
    private const float MANDATORY_CHASE_DURATION = 3f;
    #endregion
    
    private float _chaseStartTime;
    
    public MandatoryChaseState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.MandatoryChase;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;
        
        // Attiva il flag: da MandatoryChase in poi, sto inseguendo per rumore
        _enemyAI.IsNoiseChaseActive = true;

        // Stop any playing audio
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        _chaseStartTime = Time.time;
        
        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
        
        // Play state entry phrase
        if (_enemyData.MandatoryChaseState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.MandatoryChaseState_Phrases);
        }
        
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
        
        // Always chase player during mandatory duration
        _agent.SetDestination(_enemyAI.player.position);
        
        // Check attack range
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        if (distToPlayer <= _enemyData.AttackRange)
        {
            _fsm.SwitchState(_fsm.attackState);
            return;
        }
        
        // Check if mandatory duration has elapsed
        if (Time.time - _chaseStartTime >= MANDATORY_CHASE_DURATION)
        {
            // After 3 seconds, check conditions:
            // SE ci sono ancora condizioni per inseguire → vai in Chase / ChaseDistance
            if (attraction >= _enemyData.NoiseLoseThreshold || distToPlayer <= _enemyData.D_enter)
            {
                if (distToPlayer <= _enemyData.D_enter)
                {
                    // Vicino → chase per distanza
                    _fsm.SwitchState(_fsm.chaseDistanceState);
                }
                else
                {
                    // Lontano ma attratto → chase normale
                    _fsm.SwitchState(_fsm.chaseState);
                }
                return;
            }

            // SE NON ci sono più motivi per inseguire:
            // NON andare in LostTargetState (non c'è stato un vero chase).
            // Se hai un sound position valido → vai a CheckSound (o al limite StandAndExaminate),
            // altrimenti torna in Patrol.
            Vector3 soundPos = _enemyAI.GetSoundPosition();
            if (soundPos != Vector3.zero)
            {
                _fsm.SwitchState(_fsm.checkSoundState);
            }
            else
            {
                _fsm.SwitchState(_fsm.patrolState);
            }
        }
    }

    public override void Exit()
    {
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}

