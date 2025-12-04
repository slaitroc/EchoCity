
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{

    #region Constants
    protected string _LOG_TAG = "ENEMY STATE";
    protected string _LOG_COLOR = "#ff0000ff";
    #endregion 
    protected EnemyAI _enemyAI;
    protected EnemyFSM _fsm;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected SOEnemyData _enemyData;

    protected int _animSpeedParameter = Animator.StringToHash("Speed");
    protected int _animIsAttacking = Animator.StringToHash("isAttacking");

    public EnemyState(EnemyAI enemyAI, EnemyFSM fsm)
    {
        this._enemyAI = enemyAI;
        this._fsm = fsm;
        this._agent = enemyAI.agent;
        this._animator = enemyAI.animator;
        this._enemyData = enemyAI.enemyData;
    }

    public abstract void Enter();
    public abstract void Update(float attraction);
    public abstract void Exit();

    /// From this point onward, the methods represent the state-specific behaviors 
    /// that must be invoked by the EnemyAI. These methods are either called in EnemyAI's method that are subscribed 
    /// to events (Handler) or require the EnemyAI to trigger 
    /// an event or a specific action based on their return values.
    public abstract bool OnPlayerHit();

    #region Animation Helper Methods
    
    /// <summary>
    /// Plays the StandAndExamine animation using trigger.
    /// Resets other special animation triggers and sets Speed to 0.
    /// </summary>
    protected void PlayStandAndExamineAnimation()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Trig_LostTarget");
        _animator.ResetTrigger("Trig_Confused");
        _animator.SetTrigger("Trig_StandAndExamine");
        _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
    }

    /// <summary>
    /// Plays the LostTarget animation using trigger.
    /// Resets other special animation triggers and sets Speed to 0.
    /// </summary>
    protected void PlayLostTargetAnimation()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Trig_StandAndExamine");
        _animator.ResetTrigger("Trig_Confused");
        _animator.SetTrigger("Trig_LostTarget");
        _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
    }

    /// <summary>
    /// Plays the Confused animation using trigger.
    /// Resets other special animation triggers and sets Speed to 0.
    /// </summary>
    protected void PlayConfusedAnimation()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Trig_StandAndExamine");
        _animator.ResetTrigger("Trig_LostTarget");
        _animator.SetTrigger("Trig_Confused");
        _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
    }
    
    #endregion
}