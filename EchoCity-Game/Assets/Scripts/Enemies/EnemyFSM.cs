[System.Serializable]
public class EnemyFSM
{
#pragma warning disable CS0414
    #region Constants
    private string _LOG_TAG = "ENEMY FSM";
    private string _LOG_COLOR = "#ff0000ff";
    #endregion
#pragma warning restore CS0414

    public EnemyState CurrentState { get; private set; }
    private EnemyAI enemyAI;
    public readonly PatrolEnemyState patrolState;
    public readonly ChaseEnemyState chaseState;
    public readonly AttackEnemyState attackState;


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