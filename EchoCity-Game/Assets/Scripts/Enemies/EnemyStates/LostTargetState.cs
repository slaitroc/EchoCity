using UnityEngine;

/// <summary>
/// State of LOSS AFTER a real chase.
/// Enemy has already confirmed player's existence (has seen/chased the player).
/// Has done Chase, ChaseDistance, or MandatoryChase.
/// Phrases like: "So che eri qui... ti ritroverò.".
/// Always entered from chase states (MandatoryChase, Chase, ChaseDistance).
/// After LostTargetState → PatrolState, HasConfirmedPlayer is reset to false.
/// </summary>
public class LostTargetState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "LOST TARGET STATE";
    private const float LOST_TARGET_DURATION = 2f; // Duration of "Ti ritroverò" animation/phrase
    #endregion
    
    private float _stateStartTime;
    private bool _hasPlayedPhrase = false;
    
    public LostTargetState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.LostTarget;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;
        
        // Reset noise chase flag: quando il nemico dice "ti ritroverò" e entra in questo stato, 
        // ha perso definitivamente il target → il noise-chase si considera chiuso
        _enemyAI.IsNoiseChaseActive = false;

        // Play LostTarget animation using trigger
        PlayLostTargetAnimation();

        _stateStartTime = Time.time;
        _hasPlayedPhrase = false;
        
        // Stop agent
        _agent.isStopped = true;
        _agent.ResetPath();
        
        // Play state entry phrase
        if (_enemyData.LostTargetState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.LostTargetState_Phrases);
        }
        
        // Emit investigation sound (for echolocation system)
        EmitInvestigationSound();
        
        // Set animator to idle/defeated animation
        _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        // Stand still during "Ti ritroverò" phrase
        _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
        
        // Wait for duration to complete
        if (Time.time - _stateStartTime >= LOST_TARGET_DURATION)
        {
            // After phrase, check for confusing sound source
            if (_enemyAI.ShouldBeDistractedByConfusingSound())
            {
                _fsm.SwitchState(_fsm.gettingConfusedState);
            }
            else
            {
                // Return to patrol after phrase
                // Select the closest patrol area before returning to patrol
                _enemyAI.SelectClosestPatrolArea();
                _fsm.SwitchState(_fsm.patrolState);
            }
        }
    }

    public override void Exit()
    {
        _agent.isStopped = false;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
    
    /// <summary>
    /// Emits investigation sound using parameters from SOEnemyData.
    /// Called when entering LostTargetState.
    /// Uses enemySoundEmissionEvent from EnemyAI (reference only, no logic in EnemyAI).
    /// </summary>
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

