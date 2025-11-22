using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    #region Constants 
    private string _LOG_TAG = "ENEMY AI";
    private string _LOG_COLOR = "#ff0000ff";
    #endregion
    public enum State { Patrol, Chase }

    #region  Serialized Fields

    [Header("Invoking Events")]
    [SerializeField] private SOEnemyIAEvent playerHitEvent;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public SOEnemyData enemyData;
    public Transform player;
    public Transform[] waypoints;
    public AttackRangeDetector attackRangeDetector;

    [Header("FSM")]
    private EnemyFSM _fsm;
    public EnemyStatesEnum currentState;
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) Log.D("No player found for EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        else player = p.transform;

        if (enemyData == null)
        {
            Log.E("No SOEnemyData assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.E("No waypoints assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        if (attackRangeDetector == null)
        {
            Log.E("No AttackRangeDetector assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        
        _fsm = new EnemyFSM(this);
        _fsm.Initialize();
    }
    void Update()
    {
        _fsm.Update(Vector3.Distance(transform.position, player.position));
    }

    public void RaisePlayerHitEvent()
    {
        _fsm.attackState.PlayerHitHandler(this);
        playerHitEvent?.RaiseEvent(this);
    }

    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.ChaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.LoseRange);

        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere(transform.position, enemyData.KillRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
    }
}