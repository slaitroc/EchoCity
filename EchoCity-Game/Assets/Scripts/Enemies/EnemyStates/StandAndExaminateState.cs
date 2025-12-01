using UnityEngine;

/// <summary>
/// State where enemy stands at the sound position and examines the area.
/// Plays investigation phrases and checks if player is nearby or if attraction rises.
/// </summary>
public class StandAndExaminateState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "STAND AND EXAMINATE STATE";
    #endregion
    
    private Vector3 _examinationPosition;
    private float _examinationStartTime;
    private bool _hasPlayedPhrase = false;
    private float _lastPhrasePlayTime = 0f;
    private const float EXAMINATION_DURATION = 7f; // How long to examine before returning to patrol
    private const float PLAYER_DETECTION_RANGE = 3f; // If player is this close, start chasing
    private const float PHRASE_COOLDOWN = 3f; // Minimum time between playing phrases (prevents spam)
    
    public StandAndExaminateState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.StandAndExaminate;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Store examination position (sound position)
        _examinationPosition = _enemyAI.GetSoundPosition();
        if (_examinationPosition == Vector3.zero)
        {
            // No sound position - return to patrol
            Log.W("StandAndExaminateState entered but no sound position available. Returning to patrol.", _LOG_COLOR, _LOG_TAG);
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        _examinationStartTime = Time.time;
        
        // Stop agent and look around
        _agent.isStopped = true;
        _agent.ResetPath();
        
        // Play investigation phrase (audio clip) only if cooldown has passed
        // This prevents audio from playing too frequently if state is re-entered
        if (Time.time - _lastPhrasePlayTime >= PHRASE_COOLDOWN)
        {
            _enemyAI.PlayInvestigationPhrase();
            _hasPlayedPhrase = true;
            _lastPhrasePlayTime = Time.time;
        }
        
        // Emit investigation sound (for echolocation system)
        EmitInvestigationSound();
    }

    public override void Update(float attraction)
    {
        // Stand still during examination
        _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);

        // PRIORITY 1: If attraction > 1.0, ALWAYS resume chase of PLAYER
        if (attraction >= _enemyData.NoiseThreshold)
        {
            _fsm.SwitchState(_fsm.chaseState);
            return;
        }

        // PRIORITY 2: Check if player is very close (<= 3m) - start chasing
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        
        if (distToPlayer <= PLAYER_DETECTION_RANGE)
        {
            // Player is close - start chasing PLAYER
            _fsm.SwitchState(_fsm.chaseState);
            return;
        }

        // Rotate to look around (optional - can be enhanced with actual rotation logic)
        // For now, just look towards player direction
        Vector3 dirToPlayer = _enemyAI.player.position - _enemyAI.transform.position;
        dirToPlayer.y = 0f;
        if (dirToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            _enemyAI.transform.rotation = Quaternion.Slerp(
                _enemyAI.transform.rotation,
                targetRot,
                2f * Time.deltaTime
            );
        }

        // Check if examination duration elapsed
        if (Time.time - _examinationStartTime >= EXAMINATION_DURATION)
        {
            // Examination complete - return to patrol
            // Reset attraction since we didn't find anything
            _enemyAI.ResetAttraction();
            // Clear the last chase action position since investigation is complete
            _enemyAI.ClearLastChaseActionPosition();
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }
    }

    public override void Exit()
    {
        // Stop any playing audio when exiting this state
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }
        
        // Restore agent movement
        _agent.isStopped = false;
        _agent.speed = _enemyData.ChaseSpeed; // Restore speed for chase states
        
        // Reset flags
        _hasPlayedPhrase = false;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
    
    /// <summary>
    /// Emits investigation sound using parameters from SOEnemyData.
    /// Called when entering StandAndExaminateState.
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

