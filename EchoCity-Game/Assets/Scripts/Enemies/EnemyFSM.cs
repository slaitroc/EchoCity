using UnityEngine;

[System.Serializable]
public class EnemyFSM
{
    public EnemyState CurrentState { get; private set; }
    private EnemyAI enemyAI;
    
    // All states
    public PatrolEnemyState patrolState;
    public MandatoryChaseState mandatoryChaseState;
    public ChaseEnemyState chaseState;
    public ChaseDistanceState chaseDistanceState;
    public AttackEnemyState attackState;
    public CheckSoundState checkSoundState;
    public StandAndExaminateState standAndExaminateState;
    public LostTargetState lostTargetState;
    public GettingConfusedState gettingConfusedState;

    public EnemyFSM(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        patrolState = new PatrolEnemyState(enemyAI, this);
        mandatoryChaseState = new MandatoryChaseState(enemyAI, this);
        chaseState = new ChaseEnemyState(enemyAI, this);
        chaseDistanceState = new ChaseDistanceState(enemyAI, this);
        attackState = new AttackEnemyState(enemyAI, this);
        checkSoundState = new CheckSoundState(enemyAI, this);
        standAndExaminateState = new StandAndExaminateState(enemyAI, this);
        lostTargetState = new LostTargetState(enemyAI, this);
        gettingConfusedState = new GettingConfusedState(enemyAI, this);
    }

    public void Initialize()
    {
        CurrentState = patrolState;
        Log.D($"[{enemyAI.name}] FSM Initialized: Starting in Patrol state", "#00ff00ff", "ENEMY FSM");
        CurrentState.Enter();
    }

    public void Update(float attraction)
    {
        if (enemyAI.enemyData == null)
        {
            CurrentState.Update(attraction);
            return;
        }
        
        float distToPlayer = Vector3.Distance(enemyAI.transform.position, enemyAI.player.position);
        
        // GLOBAL TRIGGER 1: MandatoryChase
        // From ANY state (except if already in MandatoryChaseState):
        // If A >= A_enter (1.0) → MandatoryChaseState
        if (CurrentState != mandatoryChaseState && attraction >= enemyAI.enemyData.NoiseThreshold)
        {
            SwitchState(mandatoryChaseState);
            return;
        }
        
        // GLOBAL TRIGGER 2: ChaseDistance
        // From ANY non-chase state (Patrol, CheckSound, StandAndExamine, LostTarget, GettingConfused):
        // If d <= D_enter (10m) → ChaseDistanceState
        // Note: LostTargetState can be interrupted if player gets too close
        bool isNonChaseState = CurrentState == patrolState || 
                               CurrentState == checkSoundState || 
                               CurrentState == standAndExaminateState || 
                               CurrentState == lostTargetState ||
                               CurrentState == gettingConfusedState;
        
        if (isNonChaseState && distToPlayer <= enemyAI.enemyData.D_enter)
        {
            SwitchState(chaseDistanceState);
            return;
        }
        
        // Update current state
        CurrentState.Update(attraction);
    }

    public void SwitchState(EnemyState newState)
    {
        string oldStateName = GetStateName(CurrentState);
        string newStateName = GetStateName(newState);
        
        // Log state change
        Log.D($"[{enemyAI.name}] State Change: {oldStateName} → {newStateName}", "#00ff00ff", "ENEMY FSM");
        
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    
    /// <summary>
    /// Gets the name of a state for logging purposes
    /// </summary>
    private string GetStateName(EnemyState state)
    {
        if (state == null) return "NULL";
        if (state == patrolState) return "Patrol";
        if (state == mandatoryChaseState) return "MandatoryChase";
        if (state == chaseState) return "Chase";
        if (state == chaseDistanceState) return "ChaseDistance";
        if (state == attackState) return "Attack";
        if (state == checkSoundState) return "CheckSound";
        if (state == standAndExaminateState) return "StandAndExamine";
        if (state == lostTargetState) return "LostTarget";
        if (state == gettingConfusedState) return "GettingConfused";
        return state.GetType().Name;
    }
}