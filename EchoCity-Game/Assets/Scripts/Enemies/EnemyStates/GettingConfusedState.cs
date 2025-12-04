using UnityEngine;

/// <summary>
/// GettingConfusedState: enemy is attracted to a confusing sound source (radio, speaker, etc.).
/// Enemy moves to the source, stops there, and becomes confused for a duration.
/// Can only enter from non-chase states (Patrol, CheckSound, StandAndExamine, LostTarget).
/// </summary>
public class GettingConfusedState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "GETTING CONFUSED STATE";
    private const float ARRIVAL_DISTANCE = 2f; // Distance threshold to consider "arrived" at confusing sound
    #endregion
    
    private IConfusingSoundSource _targetSource;
    private Vector3 _targetPosition;
    private bool _hasArrived = false;
    private float _confusionStartTime;
    private bool _isConfused = false;
    
    public GettingConfusedState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.GettingConfused;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Play Confused animation using trigger
        PlayConfusedAnimation();

        // Stop any playing audio
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        // Find nearest confusing sound source
        _targetSource = _enemyAI.FindNearestConfusingSoundSource();
        
        if (_targetSource == null || !_targetSource.IsActive)
        {
            // No active confusing sound source - return to patrol
            Log.W("GettingConfusedState entered but no active confusing sound source found. Returning to patrol.", _LOG_COLOR, _LOG_TAG);
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        _targetPosition = _targetSource.Position;
        _hasArrived = false;
        _isConfused = false;
        
        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f; // No stopping distance, we want to reach the exact position
        _agent.SetDestination(_targetPosition);
        
        // Set animator to walking/curious animation
        _animator.SetFloat(_animSpeedParameter, 0.7f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        // Check if source is still active
        if (_targetSource == null || !_targetSource.IsActive)
        {
            // Source was deactivated - exit state
            ExitGettingConfused();
            return;
        }
        
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        
        // Check if player is too close - switch to ChaseDistance
        if (distToPlayer <= _enemyData.D_enter)
        {
            _fsm.SwitchState(_fsm.chaseDistanceState);
            return;
        }
        
        // Check if attraction rose too high - switch to MandatoryChase
        if (attraction >= _enemyData.NoiseThreshold)
        {
            _fsm.SwitchState(_fsm.mandatoryChaseState);
            return;
        }
        
        if (!_hasArrived)
        {
            // Moving towards confusing sound source
            _animator.SetFloat(_animSpeedParameter, 0.7f, 0.2f, Time.deltaTime);
            
            // Update target position in case source moved
            _targetPosition = _targetSource.Position;
            _agent.SetDestination(_targetPosition);
            
            // Check if arrived
            float distanceToTarget = Vector3.Distance(_enemyAI.transform.position, _targetPosition);
            if (distanceToTarget <= ARRIVAL_DISTANCE)
            {
                _hasArrived = true;
                _isConfused = true;
                _confusionStartTime = Time.time;
                
                // Stop agent and start confused animation
                _agent.isStopped = true;
                _agent.ResetPath();
                _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
                
                // TODO: Trigger confused animation (head swaying, body wobbling)
                // _animator.SetTrigger("Confused");
            }
        }
        else
        {
            // Arrived and confused - stay still
            _animator.SetFloat(_animSpeedParameter, 0f, 0.2f, Time.deltaTime);
            
            // Check if confusion duration elapsed
            if (Time.time - _confusionStartTime >= _enemyData.ConfusingSoundDuration)
            {
                ExitGettingConfused();
            }
        }
    }
    
    /// <summary>
    /// Handles exit from confused state based on conditions
    /// </summary>
    private void ExitGettingConfused()
    {
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        float attraction = _enemyAI.GetAttraction();
        
        // Exit conditions (in order of priority):
        // 1. If player is close (d <= D_enter) → ChaseDistance
        if (distToPlayer <= _enemyData.D_enter)
        {
            _fsm.SwitchState(_fsm.chaseDistanceState);
        }
        // 2. If attraction is high (A >= A_enter) → MandatoryChase
        else if (attraction >= _enemyData.NoiseThreshold)
        {
            _fsm.SwitchState(_fsm.mandatoryChaseState);
        }
        // 3. Otherwise → Patrol
        else
        {
            _fsm.SwitchState(_fsm.patrolState);
        }
    }

    public override void Exit()
    {
        _agent.isStopped = false;
        _hasArrived = false;
        _isConfused = false;
        _targetSource = null;
    }

    public override bool OnPlayerHit()
    {
        return false;
    }
}

