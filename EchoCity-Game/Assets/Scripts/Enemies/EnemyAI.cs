using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    #region Constants 
    private const string TAG = "ENEMY_AI";
    #endregion
    public enum State { Patrol, Chase }

    #region  Serialized Fields
    [Header("References")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] public SOEnemyData enemyData;

    [SerializeField] public Transform player;
    [SerializeField] public Transform[] waypoints;


    [Header("FSM")]
    private EnemyFSM _fsm;
    public EnemyStatesEnum currentState;
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) Log.D("No player found for EnemyAI on " + gameObject.name, "red", TAG);
        else player = p.transform;

        if (enemyData == null)
        {
            Log.E("No SOEnemyData assigned to EnemyAI on " + gameObject.name, "red", TAG);
        }
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.E("No waypoints assigned to EnemyAI on " + gameObject.name, "red", TAG);
        }
    }

    void Start()
    {
        _fsm = new EnemyFSM(this);
        _fsm.Initialize();
    }

    void Update()
    {
        _fsm.Update(Vector3.Distance(transform.position, player.position));
    }
    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.ChaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.LoseRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, enemyData.KillRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
    }
}