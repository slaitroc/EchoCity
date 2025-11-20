using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase }

    #region  Serialized Fields
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] public SOEnemyData enemyData;

    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;

    [Header("Animator Parameters")]
    [SerializeField] private State currentState = State.Patrol;
    [SerializeField] private int currentWaypointIndex = 0;
    #endregion

    #region Private Fields

    private bool _isWaitingAtWaypoint = false;
    private bool _isAttacking = false;
    private float _lastAttackTime = -Mathf.Infinity;
    private float _waitTimer = 0f;

    private Coroutine attackRoutine;


    private int _speedParameter = Animator.StringToHash("Speed");
    private int _idleParameter = Animator.StringToHash("IsIdle");
    private int _chaseParameter = Animator.StringToHash("IsChasing");
    private int _attackTrigger = Animator.StringToHash("Attack");
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) Log.D("No player found for EnemyAI on " + gameObject.name);
        else player = p.transform;

        if (enemyData == null)
        {
            Log.D("No SOEnemyData assigned to EnemyAI on " + gameObject.name);
        }
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.D("No waypoints assigned to EnemyAI on " + gameObject.name);
            return;
        }
        agent.speed = enemyData.PatrolSpeed;
        agent.stoppingDistance = 0f;
        GotoNextWaypoint();
    }

    void Update()
    {
        if (player == null)
        {
            UpdateAnimator(0f);
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                if (distToPlayer < enemyData.ChaseRange && !_isAttacking) SwitchToChase();
                else PatrolUpdate();
                break;
            case State.Chase:
                if (distToPlayer > enemyData.LoseRange && !_isAttacking) SwitchToPatrol();
                else ChaseUpdate(distToPlayer);
                break;
        }
        float currentSpeed = agent != null ? agent.velocity.magnitude : 0f;
        UpdateAnimator(currentSpeed);
    }

    void UpdateAnimator(float worldSpeed)
    {
        if (animator == null) return;

        // If attacking, speed is zero and not idle or chasing
        if (_isAttacking)
        {
            animator.SetFloat(_speedParameter, 0f);
            animator.SetBool(_idleParameter, false);
            animator.SetBool(_chaseParameter, false);
            return;
        }

        float normalizedSpeed = 0f;
        float maxSpeed = Mathf.Max(enemyData.PatrolSpeed, enemyData.ChaseSpeed);

        if (agent != null && maxSpeed > 0.01f)
        {
            normalizedSpeed = worldSpeed / maxSpeed;   // worldSpeed è agent.velocity.magnitude
            normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        }
        animator.SetFloat(_speedParameter, normalizedSpeed);
        animator.SetBool(_idleParameter, IsIdle());
        animator.SetBool(_chaseParameter, currentState == State.Chase && !_isAttacking);
    }

    bool IsIdle()
    {
        if (agent == null)
            return currentState == State.Patrol;

        if (_isAttacking) return false;
        if (currentState == State.Patrol && (_isWaitingAtWaypoint || agent.velocity.sqrMagnitude < 0.01f))
            return true;
        return agent.isStopped && agent.velocity.sqrMagnitude < 0.01f;
    }

    // ------------------- STATI -------------------

    void SwitchToPatrol()
    {
        CancelAttack();
        currentState = State.Patrol;
        agent.speed = enemyData.PatrolSpeed;
        agent.stoppingDistance = 0f;
        agent.isStopped = false;
        if (!_isWaitingAtWaypoint)
            GotoNextWaypoint();
    }

    void SwitchToChase()
    {
        CancelWait();
        currentState = State.Chase;
        agent.speed = enemyData.ChaseSpeed;
        agent.stoppingDistance = Mathf.Clamp(enemyData.AttackRange * 0.5f, 0.1f, enemyData.AttackRange);
        agent.isStopped = false;
    }

    void PatrolUpdate()
    {
        if (_isWaitingAtWaypoint)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaitingAtWaypoint = false;
                agent.isStopped = false;
                GotoNextWaypoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= enemyData.WaypointArrivalThreshold)
        {
            StartWaitAtWaypoint();
        }
    }

    void StartWaitAtWaypoint()
    {
        if (waypoints.Length == 0) return;

        _isWaitingAtWaypoint = true;
        _waitTimer = enemyData.WaypointPauseDuration;
        agent.isStopped = true;
        agent.ResetPath();
    }

    void CancelWait()
    {
        if (!_isWaitingAtWaypoint) return;

        _isWaitingAtWaypoint = false;
        _waitTimer = 0f;
        agent.isStopped = false;
    }

    void ChaseUpdate(float distToPlayer)
    {
        if (_isAttacking) return;
        if (distToPlayer <= enemyData.AttackRange) TryAttack(distToPlayer);
        else
        {
            agent.isStopped = false;
            if (player != null) agent.SetDestination(player.position);
        }
    }

    void TryAttack(float distToPlayer)
    {
        if (Time.time < _lastAttackTime + enemyData.AttackCoolDown)
        {
            agent.isStopped = false;
            if (player != null) agent.SetDestination(player.position);
            return;
        }

        _lastAttackTime = Time.time;
        _isAttacking = true;
        agent.isStopped = true;          // FERMO
        agent.velocity = Vector3.zero;   // STOP ASSOLUTO
        agent.ResetPath();               // NESSUNA DESTINAZIONE

        // gira verso player
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position);
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation;
            }
        }
        animator.SetTrigger(_attackTrigger);

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackRoutine());

        if (distToPlayer <= enemyData.KillRange)
        {
            KillPlayer();
        }
    }

    IEnumerator AttackRoutine()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(enemyData.AttackDuration);

        _isAttacking = false;
        agent.isStopped = false;  // torna a muoversi
        attackRoutine = null;
    }

    void CancelAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        _isAttacking = false;
        agent.isStopped = false;
    }

    void GotoNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    // ------------------- ATTACCO -------------------
    void KillPlayer()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        //TODO trigger events
    }

    // ------------------- DEBUG -------------------
    void OnDrawGizmosSelected()
    {
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