using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ECHO CITY/ENEMY/EnemyDataSO")]
public class SOEnemyData : ScriptableObject
{
    [Header("Ranges")]
    [Tooltip("Distance threshold: within this distance (10m), enemy always chases regardless of attraction")]
    public float D_enter = 10f;
    
    [Tooltip("Distance threshold: beyond this distance (15m), enemy considers player truly lost")]
    public float D_exit = 15f;
    
    [Tooltip("Attack range: distance at which enemy can attack player")]
    public float AttackRange = 3f;

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

    [Tooltip("Noise threshold for continuing chase: enemy continues chasing PLAYER if attraction > 0.8")]
    [Min(0f)]
    public float NoiseLoseThreshold = 0.8f;

    [Tooltip("Minimum chase duration: DEPRECATED - not used in new state system. MandatoryChaseState has fixed 3s duration. Kept for backward compatibility.")]
    [Min(0f)]
    public float MinChaseDuration = 5f;


    [Tooltip("Intensity factor for noise calculation")]
    [Min(0f)]
    public float NoiseIntensityFactor = 1f;

    [Tooltip("Range factor for distance calculation")]
    [Min(0f)]
    public float NoiseRangeFactor = 1f;

    [Tooltip("Decay exponent for distance falloff (higher = faster falloff)")]
    [Range(0.1f, 5f)]
    public float NoiseDistanceDecay = 0.05f;

    [Tooltip("Rate at which attraction decays per second when no sound is active")]
    [Min(0f)]
    public float NoiseDecayRate = 0.05f;

    [Header("Voice Lines - State Entry Phrases")]
    [Tooltip("SoundSource for phrases when entering MandatoryChaseState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource MandatoryChaseState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering ChaseEnemyState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource ChaseEnemyState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering ChaseDistanceState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource ChaseDistanceState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering AttackEnemyState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource AttackEnemyState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering CheckSoundState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource CheckSoundState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering StandAndExaminateState: 'Mi sembrava di sentire qualcosa...', 'Strano...'. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource StandAndExaminateState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering LostTargetState: 'So che eri qui... ti ritroverò'. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource LostTargetState_Phrases;
    
    [Tooltip("SoundSource for phrases when entering GettingConfusedState. Use RandomAudioClips array in SOSoundSource for multiple variations.")]
    public SOSoundSource GettingConfusedState_Phrases;
    
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

    [Header("Confusing Sound")]
    [Tooltip("Maximum distance at which enemy can detect confusing sound sources (R_confuse)")]
    [Min(0f)]
    public float ConfusingSoundDetectionRange = 30f;
    
    [Tooltip("Duration enemy stays confused when reaching confusing sound source (T_confuse)")]
    [Min(0.1f)]
    public float ConfusingSoundDuration = 6f;

    [Header("Attack")]
    //public float AttackCoolDown = 2f;
    public AnimationClip AttackAnimation;
    public float AttackDuration;
    public float AttackDamageDelay = 0.5f;
    public float AttackDamageWindowTime = 0.1f;
    public float AttackCoolDown = 0.2f;
    public float CoolDownRotationSpeed = 5f;

    void OnValidate()
    {
        AttackDuration = AttackAnimation != null ? AttackAnimation.length : AttackDuration;
        AttackDamageDelay = Mathf.Clamp(AttackDamageDelay, 0f, AttackDuration);
        AttackDamageWindowTime = Mathf.Clamp(AttackDamageWindowTime, 0f, AttackDuration - AttackDamageDelay);
        AttackCoolDown = Mathf.Max(AttackCoolDown, 0f);


        // Ensure AttackRange is valid (must be less than D_exit)
        AttackRange = Mathf.Clamp(AttackRange, 0f, D_exit);


        PatrolSpeed = Mathf.Clamp01(PatrolSpeed);

        // Ensure thresholds are in correct order: Lose < Chase
        NoiseLoseThreshold = Mathf.Max(0f, NoiseLoseThreshold);
        NoiseThreshold = Mathf.Max(NoiseLoseThreshold, NoiseThreshold);
    }

}