using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ECHO CITY/ENEMY/EnemyDataSO")]
public class SOEnemyData : ScriptableObject
{
    [Header("Ranges")]
    public float KillRange = 1.2f; // distance at which the player is "killed"
    public float AttackRange = 3f;
    public float ChaseRange = 15f;
    public float LoseRange = 20f;

    [Header("Velocity")]
    public float ChaseSpeed = 5.4f;

    [Range(0f, 1f)]
    public float PatrolSpeed = 0.5f;

    [Header("Patrol")]
    public float WaypointPauseDuration = 2f;
    public float WaypointArrivalThreshold = 0.3f;

    [Header("Attack")]
    public float AttackCoolDown = 2f;
    public float AttackDuration = 1f;

    void OnValidate()
    {
        KillRange = Mathf.Max(KillRange, 0.1f);
        AttackRange = Mathf.Max(AttackRange, KillRange);
        LoseRange = Mathf.Max(LoseRange, ChaseRange);

        ChaseSpeed = Mathf.Max(ChaseSpeed, PatrolSpeed);


    }

}