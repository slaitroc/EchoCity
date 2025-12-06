using UnityEngine;

/// <summary>
/// Chase state triggered by proximity: ignores attraction, chases player if within D_enter (10m).
/// Exits when player is beyond D_exit (15m), transitioning to LostTargetState.
/// </summary>
public class ChaseDistanceState : EnemyState
{
    #region Constants
    protected new string _LOG_TAG = "CHASE DISTANCE STATE";
    #endregion
    
    public ChaseDistanceState(EnemyAI enemyAI, EnemyFSM fsm) : base(enemyAI, fsm) { }

    public override void Enter()
    {
        _enemyAI.CurrentState = EnemyStatesEnum.ChaseDistance;
        _enemyAI.attackRangeDetector.attackCollider.enabled = false;
        
        // Enemy has confirmed player's existence (real chase started)
        _enemyAI.HasConfirmedPlayer = true;

        // Stop any playing audio
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
        
        // Play state entry phrase
        if (_enemyData.ChaseDistanceState_Phrases != null)
        {
            _enemyAI.PlayRandomPhrase(_enemyData.ChaseDistanceState_Phrases);
        }

        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
        
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        
        // Check attack range
        if (distToPlayer <= _enemyData.AttackRange)
        {
            _fsm.SwitchState(_fsm.attackState);
            return;
        }
        
        // Continue chasing player (ignores attraction)
        _agent.SetDestination(_enemyAI.player.position);
        
        // Exit condition: if player is beyond D_exit (15m), go to LostTargetState
        if (distToPlayer > _enemyData.D_exit)
        {
            _fsm.SwitchState(_fsm.lostTargetState);
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

