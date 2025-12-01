using UnityEngine;

/// <summary>
/// State where enemy moves towards the sound position after attraction dropped below threshold.
/// Transitions to StandAndExaminateState when reaching the sound position.
/// </summary>
public class ChaseSoundState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHASE SOUND STATE";
    #endregion
    
    private Vector3 _targetSoundPosition;
    private bool _hasTargetPosition = false;
    private const float ARRIVAL_DISTANCE = 2f; // Distance threshold to consider "arrived" at sound position
    
    public ChaseSoundState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.ChaseSound;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Stop any playing audio when entering chase sound (e.g., from StandAndExaminateState)
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
            Log.W("ChaseSoundState entered but no sound position available. Returning to patrol.", _LOG_COLOR, _LOG_TAG);
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f; // No stopping distance, we want to reach the exact position
        _agent.SetDestination(_targetSoundPosition);
        
        // Ensure animator speed is set correctly for chase
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1, 0.2f, Time.deltaTime);

        if (!_hasTargetPosition)
        {
            // No target position - should not happen, but return to patrol if it does
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        // Check if attraction rose above threshold - resume chase
        if (attraction >= _enemyData.NoiseThreshold)
        {
            _fsm.SwitchState(_fsm.chaseState);
            return;
        }

        // Check if we've reached the sound position
        float distanceToSound = Vector3.Distance(_enemyAI.transform.position, _targetSoundPosition);
        
        if (distanceToSound <= ARRIVAL_DISTANCE)
        {
            // Arrived at sound position - switch to StandAndExaminateState
            _fsm.SwitchState(_fsm.standAndExaminateState);
            return;
        }

        // Continue moving towards sound position
        // Update destination in case sound position changed (shouldn't happen, but just in case)
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
        _agent.isStopped = false;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}

