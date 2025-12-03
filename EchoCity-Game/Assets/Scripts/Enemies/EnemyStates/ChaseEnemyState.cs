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

        // Enemy has confirmed player's existence (real chase started)
        _enemyAI.HasConfirmedPlayer = true;

        // Stop any playing audio when entering chase
        if (_enemyAI.audioSource != null && _enemyAI.audioSource.isPlaying)
        {
            _enemyAI.audioSource.Stop();
        }

        _agent.speed = _enemyData.ChaseSpeed;
        _agent.isStopped = false;
        _agent.stoppingDistance = _enemyData.AttackRange;
        
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
    }

    public override void Update(float attraction)
    {
        _animator.SetFloat(_animSpeedParameter, 1f, 0.2f, Time.deltaTime);
        
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);

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
            // Il nemico ha PERSO il player dopo un vero chase → sempre LostTargetState
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