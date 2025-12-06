using UnityEngine;

/// <summary>
/// CheckSoundState: enemy moves towards the last known sound position.
/// When reaching the point:
/// - If d <= D_enter (10m) → ChaseDistanceState
/// - Otherwise → StandAndExamineState (always, regardless of attraction)
/// StandAndExamineState will decide if returning to Patrol when attraction drops.
/// </summary>
public class CheckSoundState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHECK SOUND STATE";
    private const float ARRIVAL_DISTANCE = 2f; // Distance threshold to consider "arrived" at sound position
    #endregion
    
    private Vector3 _targetSoundPosition;
    private bool _hasTargetPosition = false;
    private bool _hasArrived = false;
    private float _arrivalTime = 0f;
    
    public CheckSoundState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.CheckSound;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Stop any playing audio
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        // Get sound position (last known sound position)
        Vector3 soundPos = _enemyAI.GetSoundPosition();
        
        if (soundPos != Vector3.zero)
        {
            _targetSoundPosition = soundPos;
            _hasTargetPosition = true;
        }
        else
        {
            // No sound position available - return to patrol
            Log.W("CheckSoundState entered but no sound position available. Returning to patrol.", _LOG_COLOR, _LOG_TAG);
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f; // No stopping distance, we want to reach the exact position
        _agent.SetDestination(_targetSoundPosition);
        
        // Play state entry phrase
        if (_enemyData.CheckSoundState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.CheckSoundState_Phrases);
        }

        _hasArrived = false;
        _arrivalTime = 0f;
        
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        if (!_hasTargetPosition)
        {
            // No target position - return to patrol
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        
        // Check if we've reached the sound position
        float distanceToSound = Vector3.Distance(_enemyAI.transform.position, _targetSoundPosition);
        
        if (!_hasArrived && distanceToSound <= ARRIVAL_DISTANCE)
        {
            // Just arrived at sound position - stop and start examination timer
            _hasArrived = true;
            _arrivalTime = Time.time;
            _agent.isStopped = true;
            _agent.ResetPath();
            _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
        }
        
        if (_hasArrived)
        {
            // Stay still during examination
            _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
            

            // Examination complete - check conditions
            // If d <= D_enter (10m) → ChaseDistanceState (ti ha trovato vicino → entra in chase)
            if (distToPlayer <= _enemyData.D_enter)
            {
                // Se arrivo da CheckSound verso un chase, è sempre perché ho investigato un SUONO → quindi è un noise-chase
                _enemyAI.IsNoiseChaseActive = true;
                _fsm.SwitchState(_fsm.chaseDistanceState);
                return;
            }
            
            // Check if attraction is high enough to trigger chase
            if (attraction >= _enemyData.NoiseThreshold)
            {
                // Se arrivo da CheckSound verso un chase, è sempre perché ho investigato un SUONO → quindi è un noise-chase
                _enemyAI.IsNoiseChaseActive = true;
                _fsm.SwitchState(_fsm.chaseState);
                return;
            }
                
            // ALTRIMENTI → entra SEMPRE in StandAndExaminateState
            // NON mandarlo più direttamente in Patrol da qui.
            // Da qui in poi sarà StandAndExaminateState a decidere se tornare a Patrol quando l'attrazione scende.
            _fsm.SwitchState(_fsm.standAndExaminateState);
            return;

        }

        // Not arrived yet - continue moving towards sound position
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);

        // Check for confusing sound source: if active and stronger than player sound, override target
        IConfusingSoundSource confusingSource = _enemyAI.FindNearestConfusingSoundSource();
        if (confusingSource != null && confusingSource.IsActive && _enemyAI.ShouldBeDistractedByConfusingSound())
        {
            // Compare distraction strength with current player attraction
            // If confusing sound is stronger, switch to GettingConfusedState
            float playerSoundStrength = attraction; // Use current attraction as player sound strength
            if (confusingSource.DistractionStrength > playerSoundStrength)
            {
                _fsm.SwitchState(_fsm.gettingConfusedState);
                return;
            }
        }

        // Continue moving towards sound position
        // Update destination in case sound position changed
        Vector3 currentSoundPos = _enemyAI.GetSoundPosition();
        if (currentSoundPos != Vector3.zero && currentSoundPos != _targetSoundPosition)
        {
            _targetSoundPosition = currentSoundPos;
            _agent.SetDestination(_targetSoundPosition);
        }
    }

    public override void Exit()
    {
        _hasTargetPosition = false;
        _hasArrived = false;
        _arrivalTime = 0f;
        _agent.isStopped = false;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}

