using UnityEngine;

public class ChaseEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHASE ENEMY STATE";
    #endregion
    
    public ChaseEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.Chase;
        
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Stop any playing audio when entering chase
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
        
        // Play state entry phrase
        if (_enemyData.ChaseEnemyState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.ChaseEnemyState_Phrases);
        }
        
        // Emit investigation sound (for echolocation system)
        EmitInvestigationSound();
 
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
        
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);

        // Confirm player only when actually close (within D_enter)
        if (!_enemyAI.HasConfirmedPlayer && distToPlayer <= _enemyData.D_enter)
        {
            _enemyAI.HasConfirmedPlayer = true;
        }

        // Check attack range (if player is close, attack)
        if (distToPlayer <= _enemyData.AttackRange)
        {
            _fsm.SwitchState(_fsm.attackState);
            return;
        }

        // Rimani in ChaseState se:
        // A >= A_exit (0.8) OPPURE d <= D_enter (10m)
        bool shouldContinueChase = attraction >= _enemyData.NoiseLoseThreshold || distToPlayer <= _enemyData.D_enter;
        
        if (shouldContinueChase)
        {
            // Continue chasing player
            _agent.SetDestination(_enemyAI.player.position);
        }
        else
        {
            // Esci dal ChaseState solo quando: A < 0.8 E d > 10m
            // Se è una noise-chase e non ha ancora confermato il player → CheckSoundState
            // Altrimenti → LostTargetState (ha perso il player dopo un vero chase)
            if (_enemyAI.IsNoiseChaseActive && !_enemyAI.HasConfirmedPlayer)
            {
                _fsm.SwitchState(_fsm.checkSoundState);
            }
            else
            {
                _fsm.SwitchState(_fsm.lostTargetState);
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
    
    private void EmitInvestigationSound()
    {
        // Get event from EnemyAI (just a reference, no logic)
        if (_enemyAI.enemySoundEmissionEvent == null || _enemyData == null) return;
        
        // Create sound emission data with investigation parameters
        SoundEmissionData soundData = new SoundEmissionData(
            _enemyAI.transform.position,                            // pos: enemy position
            _enemyData.InvestigationSoundRadius,                     // rad: radius
            _enemyData.InvestigationSoundIntensity,                  // intens: intensity
            _enemyData.InvestigationSoundDuration,                   // dur: duration
            _enemyData.InvestigationSoundFrequency                   // objFreq: frequency (0=Low, 1=Mid, 2=High)
        );
        
        // Raise event (logic is in the state, EnemyAI is just a reference holder)
        _enemyAI.enemySoundEmissionEvent.RaiseEvent(soundData);
    }
}