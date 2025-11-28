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

    [Header("Noise Detection")]
    [Tooltip("Noise threshold for UI display: when noise reaches this level, UI bar appears")]
    [Min(0f)]
    public float NoiseUIThreshold = 5f;

    [Tooltip("Noise threshold for chasing: when noise exceeds this value, enemy starts chasing")]
    [Min(0f)]
    public float NoiseThreshold = 10f;

    [Tooltip("Noise threshold for losing chase: when noise drops below this value while chasing, enemy stops chasing")]
    [Min(0f)]
    public float NoiseLoseThreshold = 3f;

    [Tooltip("Minimum chase duration: enemy will chase for at least this many seconds regardless of noise level")]
    [Min(0f)]
    public float MinChaseDuration = 3f;

    [Tooltip("Rate at which noise decays over time (per second)")]
    [Range(0f, 10f)]
    public float NoiseDecayRate = 1f;

    [Tooltip("Maximum distance at which noise can be perceived (beyond this, noise has no effect)")]
    [Min(0f)]
    public float MaxNoisePerceptionDistance = 50f;

    [Header("Noise Calculation (Attraction Formula)")]
    [Tooltip("Intensity factor for noise calculation")]
    [Min(0f)]
    public float NoiseIntensityFactor = 1f;

    [Tooltip("Range factor for distance calculation")]
    [Min(0f)]
    public float NoiseRangeFactor = 1f;

    [Tooltip("Decay exponent for distance falloff (higher = faster falloff)")]
    [Range(0.1f, 5f)]
    public float NoiseDistanceDecay = 1f;

    [Header("Investigation")]
    [Tooltip("Audio clips the enemy plays when arriving at noise position and finding nothing")]
    public AudioClip[] InvestigationPhrases;

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

        // Ensure thresholds are in correct order: Lose < UI < Chase
        NoiseLoseThreshold = Mathf.Max(0f, NoiseLoseThreshold);
        NoiseUIThreshold = Mathf.Max(NoiseUIThreshold, NoiseLoseThreshold);
        NoiseThreshold = Mathf.Max(NoiseThreshold, NoiseUIThreshold);
    }

}