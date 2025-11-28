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

        // Record chase start time for minimum duration
        _chaseStartTime = Time.time;
        _minChaseDurationElapsed = false;

        // Ensure we have a last noise position to investigate (should be set by PatrolEnemyState)
        // If not set, use player position as fallback
        if (!_enemyAI.HasLastNoisePosition())
        {
            _enemyAI.SetLastNoisePosition(_enemyAI.player.position);
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
    }

    public override void Update(float distToPlayer)
    {
        _animator.SetFloat(_animSpeedParameter, 1, 0.2f, Time.deltaTime);
        
        // Check if minimum chase duration has elapsed
        if (!_minChaseDurationElapsed && Time.time - _chaseStartTime >= _enemyData.MinChaseDuration)
        {
            _minChaseDurationElapsed = true;
        }

        float currentNoiseLevel = _enemyAI.GetNoiseLevel();
        bool isInvestigating = false;

        // Always chase player during minimum duration
        if (!_minChaseDurationElapsed)
        {
            _agent.SetDestination(_enemyAI.player.position);
        }
        else
        {
            // After minimum duration, check noise level
            if (currentNoiseLevel >= _enemyData.NoiseThreshold)
            {
                // Noise still high (or rised again) - continue chasing player
                _agent.SetDestination(_enemyAI.player.position);
            }
            else if (currentNoiseLevel < _enemyData.NoiseLoseThreshold)
            {
                // Noise dropped - go investigate last known noise position
                if (_enemyAI.HasLastNoisePosition())
                {
                    Vector3 noisePos = _enemyAI.GetLastNoisePosition();
                    float distToNoise = Vector3.Distance(_enemyAI.transform.position, noisePos);
                    
                    // If we're close to the noise position, check if we found something
                    if (distToNoise < 2f)
                    {
                        // Check conditions: player not there AND noise still low
                        float distToPlayerAtNoisePos = Vector3.Distance(noisePos, _enemyAI.player.position);
                        bool playerNotThere = distToPlayerAtNoisePos > _enemyData.AttackRange;
                        bool noiseStillLow = currentNoiseLevel < _enemyData.NoiseLoseThreshold;
                        
                        if (playerNotThere && noiseStillLow)
                        {
                            // Found nothing - play investigation phrase and return to patrol
                            _enemyAI.PlayInvestigationPhrase();
                            _enemyAI.ClearLastNoisePosition();
                            _fsm.SwitchState(_fsm.patrolState);
                            return;
                        }
                        else
                        {
                            // Player is there or noise rised - continue chasing
                            _agent.SetDestination(_enemyAI.player.position);
                            return;
                        }
                    }
                    
                    // Otherwise, continue investigating the noise position (maintain ChaseSpeed)
                    _agent.SetDestination(noisePos);
                    isInvestigating = true;
                }
                else
                {
                    // No noise position to investigate, return to patrol
                    _fsm.SwitchState(_fsm.patrolState);
                    return;
                }
            }
            else
            {
                // Noise between NoiseLoseThreshold and NoiseThreshold - continue chasing player
                _agent.SetDestination(_enemyAI.player.position);
            }
        }

        // Check if we should lose the chase: BOTH conditions must be true
        // 1. Noise below NoiseLoseThreshold
        // 2. Distance greater than LoseRange
        // Only check after minimum duration
        if (_minChaseDurationElapsed && 
            currentNoiseLevel < _enemyData.NoiseLoseThreshold && 
            distToPlayer > _enemyData.LoseRange && 
            !isInvestigating)
        {
            _fsm.SwitchState(_fsm.patrolState);
            return;
        }

        // Check attack range (only if not investigating)
        if (!isInvestigating && distToPlayer <= _enemyData.AttackRange)
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