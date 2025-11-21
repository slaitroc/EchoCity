
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public abstract class EnemyState
{
    protected EnemyAI enemyAI;
    protected EnemyFSM fsm;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected SOEnemyData enemyData;

    protected int _speedParameter = Animator.StringToHash("Speed");
    protected int _attackTrigger = Animator.StringToHash("Attack");

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