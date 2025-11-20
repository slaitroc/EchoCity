using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase }

    #region  Seriralized Fields
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Animator animator;

    [Header("Ranges")]
    [SerializeField] private float killRange = 1.2f; // distance at which the player is "killed"
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float loseRange = 20f;

    [Header("Velocity")]
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 5.4f;

    [Header("Patrol")]
    [SerializeField] private float waypointPauseDuration = 2f;
    [SerializeField] private float waypointArrivalThreshold = 0.3f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDuration = 1f;

    [Header("Animator Parameters")]
    [SerializeField] private State currentState = State.Patrol;
    [SerializeField] private int currentWaypointIndex = 0;
    #endregion

    #region Private Fields
    private int speedParameter = Animator.StringToHash("Speed");
    private int idleParameter = Animator.StringToHash("IsIdle");
    private int chaseParameter = Animator.StringToHash("IsChasing");
    private int attackTrigger = Animator.StringToHash("Attack");
    private bool isWaitingAtWaypoint = false;
    private float waitTimer = 0f;
    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private Coroutine attackRoutine;
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
    }

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0f;
            GotoNextWaypoint();
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
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
                if (distToPlayer < chaseRange && !isAttacking) SwitchToChase();
                else PatrolUpdate();
                break;
            case State.Chase:
                if (distToPlayer > loseRange && !isAttacking) SwitchToPatrol();
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
        if (isAttacking)
        {
            animator.SetFloat(speedParameter, 0f);
            animator.SetBool(idleParameter, false);
            animator.SetBool(chaseParameter, false);
            return;
        }

        float normalizedSpeed = 0f;
        float maxSpeed = Mathf.Max(patrolSpeed, chaseSpeed);

        if (agent != null && maxSpeed > 0.01f)
        {
            normalizedSpeed = worldSpeed / maxSpeed;   // worldSpeed è agent.velocity.magnitude
            normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        }
        animator.SetFloat(speedParameter, normalizedSpeed);
        animator.SetBool(idleParameter, IsIdle());
        animator.SetBool(chaseParameter, currentState == State.Chase && !isAttacking);
    }

    bool IsIdle()
    {
        if (agent == null)
            return currentState == State.Patrol;

        if (isAttacking) return false;
        if (currentState == State.Patrol && (isWaitingAtWaypoint || agent.velocity.sqrMagnitude < 0.01f))
            return true;
        return agent.isStopped && agent.velocity.sqrMagnitude < 0.01f;
    }

    // ------------------- STATI -------------------

    void SwitchToPatrol()
    {
        CancelAttack();
        currentState = State.Patrol;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;
        agent.isStopped = false;
        if (!isWaitingAtWaypoint)
            GotoNextWaypoint();
    }

    void SwitchToChase()
    {
        CancelWait();
        currentState = State.Chase;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = Mathf.Clamp(attackRange * 0.5f, 0.1f, attackRange);
        agent.isStopped = false;
    }

    void PatrolUpdate()
    {
        if (isWaitingAtWaypoint)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaitingAtWaypoint = false;
                agent.isStopped = false;
                GotoNextWaypoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= waypointArrivalThreshold)
        {
            StartWaitAtWaypoint();
        }
    }

    void StartWaitAtWaypoint()
    {
        if (waypoints.Length == 0) return;

        isWaitingAtWaypoint = true;
        waitTimer = waypointPauseDuration;
        agent.isStopped = true;
        agent.ResetPath();
    }

    void CancelWait()
    {
        if (!isWaitingAtWaypoint) return;

        isWaitingAtWaypoint = false;
        waitTimer = 0f;
        agent.isStopped = false;
    }

    void ChaseUpdate(float distToPlayer)
    {
        if (isAttacking) return;
        if (distToPlayer <= attackRange) TryAttack(distToPlayer);
        else
        {
            agent.isStopped = false;
            if (player != null) agent.SetDestination(player.position);
        }
    }

    void TryAttack(float distToPlayer)
    {
        if (Time.time < lastAttackTime + attackCooldown)
        {
            agent.isStopped = false;
            if (player != null) agent.SetDestination(player.position);
            return;
        }

        lastAttackTime = Time.time;
        isAttacking = true;
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
        animator.SetTrigger(attackTrigger);

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackRoutine());

        if (distToPlayer <= killRange)
        {
            KillPlayer();
        }
    }

    IEnumerator AttackRoutine()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(attackDuration);
        
        isAttacking = false;
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
        isAttacking = false;
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
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, killRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}