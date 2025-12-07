using UnityEngine;

/// <summary>
/// State of SUSPICION before confirming player's existence.
/// Enemy has only HEARD something, hasn't done a real chase yet.
/// Used only when HasConfirmedPlayer == false.
/// Phrases like: "Mi è sembrato di sentire qualcosa...", "Strano...".
/// </summary>
public class StandAndExaminateState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "STAND AND EXAMINATE STATE";
    #endregion

    private Vector3 _examinationPosition;
    private float _examinationStartTime;
#pragma warning disable CS0414
    private bool _hasPlayedPhrase = false;
#pragma warning restore CS0414
    private float _lastPhrasePlayTime = 0f;
    private const float EXAMINATION_DURATION = 5f; // How long to examine before checking conditions
    private const float PHRASE_COOLDOWN = 3f; // Minimum time between playing phrases (prevents spam)

    public StandAndExaminateState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.StandAndExaminate;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Play StandAndExamine animation using trigger
        PlayStandAndExamineAnimation();

        // Verifica: questo stato è solo per sospetto PRE-CHASE
        // Se HasConfirmedPlayer == true, non dovresti essere qui (errore)
        if (_enemyAI.HasConfirmedPlayer)
        {
            Log.W("StandAndExaminateState entered but HasConfirmedPlayer is true. This should not happen. Going to LostTargetState.", _LOG_COLOR, _LOG_TAG);
            _fsm.SwitchState(_fsm.lostTargetState);
            return;
        }

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
        
        // Play state entry phrase (only if cooldown has passed to prevent spam)
        if (Time.time - _lastPhrasePlayTime >= PHRASE_COOLDOWN && _enemyData.StandAndExaminateState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.StandAndExaminateState_Phrases);
            _lastPhrasePlayTime = Time.time;
        }

        // Emit investigation sound (for echolocation system)
        EmitInvestigationSound();
    }

    public override void Update(float attraction)
    {
        // Stand still during examination
        _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);

        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);

        // Check for confusing sound source (enemy gets bored and goes to investigate)
        if (_enemyAI.ShouldBeDistractedByConfusingSound())
        {
            _fsm.SwitchState(_fsm.gettingConfusedState);
            return;
        }

        // Note: Global triggers in EnemyFSM handle:
        // - A >= 1.0 → MandatoryChaseState
        // - d <= 10m → ChaseDistanceState

        // Check if examination duration elapsed
        if (Time.time - _examinationStartTime >= EXAMINATION_DURATION)
        {
            // Examination complete - check conditions:
            // Se A < A_exit (0.8) E d > D_enter (10m) → PatrolState
            // Se A >= A_exit (0.8) E d > D_enter (10m) → rimane in StandAndExamineState
            if (attraction < _enemyData.NoiseLoseThreshold && distToPlayer > _enemyData.D_enter)
            {
                // Lost interest and player is far → return to patrol
                _enemyAI.ResetAttraction();
                _enemyAI.ClearLastChaseActionPosition();
                _fsm.SwitchState(_fsm.patrolState);
            }
            // Otherwise, continue examining (stay in state)
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

