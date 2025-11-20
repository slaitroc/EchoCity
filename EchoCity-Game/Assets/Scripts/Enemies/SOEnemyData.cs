using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ECHO CITY/ENEMY/EnemyDataSO")]
public class SOEnemyData : ScriptableObject
{
    [Header("Ranges")]
    public readonly float KillRange = 1.2f; // distance at which the player is "killed"
    public readonly float AttackRange = 3f;
    public readonly float ChaseRange = 15f;
    public readonly float LoseRange = 20f;

    [Header("Velocity")]
    public readonly float PatrolSpeed = 3.5f;
    public readonly float ChaseSpeed = 5.4f;

    [Header("Patrol")]
    public readonly float WaypointPauseDuration = 2f;
    public readonly float WaypointArrivalThreshold = 0.3f;

    [Header("Attack")]
    public readonly float AttackCoolDown = 2f;
    public readonly float AttackDuration = 1f;

}