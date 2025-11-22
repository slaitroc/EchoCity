
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{

    #region Constants
    protected string _LOG_TAG = "ENEMY STATE";
    protected string _LOG_COLOR = "#ff0000ff";
    #endregion 
    protected EnemyAI enemyAI;
    protected EnemyFSM fsm;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected SOEnemyData enemyData;

    protected int _animSpeedParameter = Animator.StringToHash("Speed");
    protected int _animIsAttacking = Animator.StringToHash("isAttacking");

    public EnemyState(EnemyAI enemyAI, EnemyFSM fsm)
    {
        this.enemyAI = enemyAI;
        this.fsm = fsm;
        this.agent = enemyAI.agent;
        this.animator = enemyAI.animator;
        this.enemyData = enemyAI.enemyData;
    }

    public abstract void Enter();
    public abstract void Update(float distToPlayer);
    public abstract void Exit();
}