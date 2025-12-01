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
    [Tooltip("Noise threshold for chasing: when noise exceeds this value (1.0), enemy starts chasing PLAYER")]
    [Min(0f)]
    public float NoiseThreshold = 1.0f;

    [Tooltip("Noise threshold for continuing chase: enemy continues chasing PLAYER if attraction > 0.8 (after min duration)")]
    [Min(0f)]
    public float NoiseLoseThreshold = 0.8f;

    [Tooltip("Minimum chase duration: enemy will chase for at least this many seconds regardless of noise level")]
    [Min(0f)]
    public float MinChaseDuration = 3f;


    [Tooltip("Intensity factor for noise calculation")]
    [Min(0f)]
    public float NoiseIntensityFactor = 1f;

    [Tooltip("Range factor for distance calculation")]
    [Min(0f)]
    public float NoiseRangeFactor = 1f;

    [Tooltip("Decay exponent for distance falloff (higher = faster falloff)")]
    [Range(0.1f, 5f)]
    public float NoiseDistanceDecay = 0.5f;

    [Tooltip("Rate at which attraction decays per second when no sound is active")]
    [Min(0f)]
    public float NoiseDecayRate = 0.5f;

    [Header("Investigation")]
    [Tooltip("Audio clips the enemy plays when arriving at noise position and finding nothing")]
    public AudioClip[] InvestigationPhrases;
    
    [Tooltip("Sound emitted by enemy when investigating (for echolocation system)")]
    [Header("Investigation Sound Emission")]
    [Min(0f)]
    public float InvestigationSoundIntensity = 0.3f;
    
    [Tooltip("Duration of investigation sound (seconds)")]
    [Min(0.1f)]
    public float InvestigationSoundDuration = 1.0f;
    
    [Tooltip("Frequency of investigation sound: 0=Low, 1=Mid, 2=High")]
    [Range(0f, 2f)]
    public float InvestigationSoundFrequency = 0f;
    
    [Tooltip("Radius of investigation sound emission")]
    [Min(0f)]
    public float InvestigationSoundRadius = 5f;

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

        // Ensure thresholds are in correct order: Lose < Chase
        NoiseLoseThreshold = Mathf.Max(0f, NoiseLoseThreshold);
        NoiseThreshold = Mathf.Max(NoiseLoseThreshold, NoiseThreshold);
    }

}