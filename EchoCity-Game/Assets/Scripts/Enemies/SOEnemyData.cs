using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ECHO CITY/ENEMY/EnemyDataSO")]
public class SOEnemyData : ScriptableObject
{
    [Header("Ranges")]
    public float ChaseRange = 15f;
    public float AttackRange = 3f;
    public float LoseRange = 20f;

    [Header("Velocity")]
    public float ChaseSpeed = 5.4f;

    [Range(0f, 1f)]
    public float PatrolSpeed = 0.5f;

    [Header("Patrol")]
    public float WaypointPauseDuration = 2f;
    public float WaypointArrivalThreshold = 0.3f;

    [Header("Attack")]
    //public float AttackCoolDown = 2f;
    public AnimationClip AttackAnimation;
    public float AttackDuration;
    public float AttackDamageDelay = 0.5f;
    public float AttackDamageWindowTime = 0.1f;
    public float AttackCoolDown = 2f;
    public float CoolDownRotationSpeed = 5f;

    void OnValidate()
    {
        AttackDuration = AttackAnimation != null ? AttackAnimation.length : AttackDuration;
        AttackDamageDelay = Mathf.Clamp(AttackDamageDelay, 0f, AttackDuration);
        AttackDamageWindowTime = Mathf.Clamp(AttackDamageWindowTime, 0f, AttackDuration - AttackDamageDelay);
        AttackCoolDown = Mathf.Max(AttackCoolDown, 0f);


        AttackRange = Mathf.Clamp(AttackRange, 0f, ChaseRange);
        LoseRange = Mathf.Max(LoseRange, ChaseRange);


        PatrolSpeed = Mathf.Clamp01(PatrolSpeed);


    }

}