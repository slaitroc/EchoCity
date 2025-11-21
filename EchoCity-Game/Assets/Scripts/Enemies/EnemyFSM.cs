[System.Serializable]
public class EnemyFSM
{
    public EnemyState CurrentState { get; private set; }
    private EnemyAI enemyAI;
    public PatrolEnemyState patrolState;
    public ChaseEnemyState chaseState;
    public AttackEnemyState attackState;


    public EnemyFSM(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
        patrolState = new PatrolEnemyState(enemyAI, this);
        chaseState = new ChaseEnemyState(enemyAI, this);
        attackState = new AttackEnemyState(enemyAI, this);
    }

    public void Initialize()
    {
        CurrentState = patrolState;
        CurrentState.Enter();
    }

    public void Update(float distToPlayer)
    {
        CurrentState.Update(distToPlayer);
    }

    public void SwitchState(EnemyState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}