using UnityEngine;

public class ChaseEnemyState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHASE ENEMY STATE";
    #endregion
    
    private float _chaseStartTime = 0f;
    private bool _minChaseDurationElapsed = false;
    
    public ChaseEnemyState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.Chase;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;

        // Stop any playing audio when entering chase (e.g., from StandAndExaminateState)
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        // Record chase start time for minimum duration
        _chaseStartTime = Time.time;
        _minChaseDurationElapsed = false;

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
        
        // Ensure animator speed is set correctly for chase
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1, 0.2f, Time.deltaTime);
        
        // Check if minimum chase duration has elapsed
        if (!_minChaseDurationElapsed && Time.time - _chaseStartTime >= _enemyData.MinChaseDuration)
        {
            _minChaseDurationElapsed = true;
        }

        // Calculate distance to player (for range check and attack)
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);

        // PRIORITY 1: If attraction > 1.0, ALWAYS chase PLAYER (regardless of duration)
        if (attraction >= _enemyData.NoiseThreshold)
        {
            _agent.SetDestination(_enemyAI.player.position);
        }
        // During minimum duration: always chase PLAYER
        else if (!_minChaseDurationElapsed)
        {
            _agent.SetDestination(_enemyAI.player.position);
        }
        // After minimum duration: check attraction and player range
        else
        {
            // PRIORITY 2: Continue chasing if attraction > 0.8 AND player in range (15m)
            if (attraction > _enemyData.NoiseLoseThreshold && distToPlayer <= _enemyData.ChaseRange)
            {
                // Continue chasing PLAYER
                _agent.SetDestination(_enemyAI.player.position);
            }
            // PRIORITY 3: If attraction < 0.8 OR player out of range, go to sound position
            else
            {
                // Switch to ChaseSoundState (will go to last known sound position)
                _fsm.SwitchState(_fsm.chaseSoundState);
                return;
            }
        }

        // Check attack range (if player is close, attack)
        if (distToPlayer <= _enemyData.AttackRange)
        {
            _fsm.SwitchState(_fsm.attackState);
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