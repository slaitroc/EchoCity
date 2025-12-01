using UnityEngine;

[System.Serializable]
public class EnemyFSM
{
    public EnemyState CurrentState { get; private set; }
    private EnemyAI enemyAI;
    public PatrolEnemyState patrolState;
    public ChaseEnemyState chaseState;
    public AttackEnemyState attackState;
    public ChaseSoundState chaseSoundState;
    public StandAndExaminateState standAndExaminateState;


    public EnemyFSM(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        patrolState = new PatrolEnemyState(enemyAI, this);
        chaseState = new ChaseEnemyState(enemyAI, this);
        attackState = new AttackEnemyState(enemyAI, this);
        chaseSoundState = new ChaseSoundState(enemyAI, this);
        standAndExaminateState = new StandAndExaminateState(enemyAI, this);
    }

    public void Initialize()
    {
        CurrentState = patrolState;
        CurrentState.Enter();
    }

    public void Update(float attraction)
    {
        // GLOBAL CONDITION: If player is within 8 meters, always chase regardless of attraction
        // This applies to ALL states (except if already in ChaseState or AttackState)
        if (CurrentState != chaseState && CurrentState != attackState)
        {
            float distToPlayer = Vector3.Distance(enemyAI.transform.position, enemyAI.player.position);
            const float PROXIMITY_CHASE_DISTANCE = 8f;
            
            if (distToPlayer <= PROXIMITY_CHASE_DISTANCE)
            {
                // Player is very close - switch to chase immediately
                SwitchState(chaseState);
                return;
            }
        }
        
        CurrentState.Update(attraction);
    }

    public void SwitchState(EnemyState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}