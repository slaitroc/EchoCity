using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ECHO CITY/ENEMY/EnemyDataSO")]
    public class SOEnemyData : ScriptableObject
    {
        [Header("FOV")]
        [SerializeField] private FOVParams fovData = new FOVParams(15f, 90f);

        [Header("Patrol")]
        [SerializeField] private float patrolSpeed = 0.5f; // Range(0f, 1f)
        [SerializeField] private float patrolSpeedDampTime = 0.3f;
        [SerializeField] private float patrolAcceleration = 2f;
        [SerializeField] private float patrolAngularSpeed = 400f;
        [SerializeField] private float waypointPauseDuration = 2f;
        [SerializeField] private float waypointArrivalThreshold = 2f;

        [Header("Chase")]
        [SerializeField] private float chaseSpeed = 5.4f;
        [SerializeField] private float chaseSpeedDampTime = 0.2f;
        [SerializeField] private float chaseAcceleration = 8f;
        [SerializeField] private float chaseAngularSpeed = 400f;
        [SerializeField] private float chaseSoundArrivalThreshold = 2f;
        [SerializeField] private float checkSoundPauseDuration = 2f;
        [SerializeField] private float minChaseDuration = 5f; //DANGER


        [Header("Attack")]
        [SerializeField] private float attackRange = 3f; // Attack range: distance at which enemy can attack player
        [SerializeField] private float attackSpeedDampTime = 0.2f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private AnimationClip attackAnimation;
        [SerializeField] private float attackDuration;
        [SerializeField] private float attackDamageDelay = 0.5f;
        [SerializeField] private float attackDamageWindowTime = 0.1f;
        [SerializeField] private float attackCoolDown = 0.2f;
        [SerializeField] private float coolDownRotationSpeed = 5f;

        [Header("Confusion")]
        [SerializeField] private float confusingSoundDetectionRange = 30f; // Maximum distance at which enemy can detect confusing sound sources (R_confuse)

        [Header("Attraction Parameters")]
        [SerializeField] private float distanceLowerBound = 1.0f;
        [SerializeField] private float aMaxIncrementPerFrame = 0.2f;
        [SerializeField] private float detectionVelocity = 1.0f;
        [SerializeField] private float at = 1.0f;
        [SerializeField] private float atMAX = 2.0f;
        [SerializeField] private float aIntensity = 1.0f;
        [SerializeField] private float aDecay = 0.08f;
        [SerializeField] private float decayGrowthRate = 0.5f;
        [SerializeField] private float aRange = 1f;

        [Header("Confusion Parameters")]
        [SerializeField] private float ct = 1.0f;
        [SerializeField] private float ctMAX = 1.0f;
        [SerializeField] private float cIntensity = 0.8f;
        [SerializeField] private float cDecayFactor = 0.08f;

        [Header("Voice Lines - State Entry Phrases")]
        [SerializeField] private SOSoundSource soundChaseStatePhrases; // entering SoundChase state phrase
        [SerializeField] private SOSoundSource playerChaseStatePhrases; // entering PlayerChase state phrase
        [SerializeField] private SOSoundSource attackStatePhrases; // entering AttackEnemy state phrase
        [SerializeField] private SOSoundSource checkSoundStatePhrases; // entering CheckSound state phrase
        [SerializeField] private SOSoundSource lostTargetPhrases; // PlayerChase -> Patrol phrase 
        [SerializeField] private SOSoundSource confusedStatePhrases; // entering Confused state phrase

        public FOVParams FOVData => fovData;

        public float PatrolSpeed => patrolSpeed;
        public float PatrolSpeedDampTime => patrolSpeedDampTime;
        public float PatrolAcceleration => patrolAcceleration;
        public float PatrolAngularSpeed => patrolAngularSpeed;
        public float WaypointPauseDuration => waypointPauseDuration;
        public float WaypointArrivalThreshold => waypointArrivalThreshold;

        public float ChaseSpeed => chaseSpeed;
        public float ChaseSpeedDampTime => chaseSpeedDampTime;
        public float ChaseAcceleration => chaseAcceleration;
        public float ChaseAngularSpeed => chaseAngularSpeed;
        public float ChaseSoundArrivalThreshold => chaseSoundArrivalThreshold;
        public float CheckSoundPauseDuration => checkSoundPauseDuration;
        public float MinChaseDuration => minChaseDuration;

        public float AttackRange => attackRange;
        public float AttackSpeedDampTime => attackSpeedDampTime;
        public float Damage => damage;
        public AnimationClip AttackAnimation => attackAnimation;
        public float AttackDuration => attackDuration;
        public float AttackDamageDelay => attackDamageDelay;
        public float AttackDamageWindowTime => attackDamageWindowTime;
        public float AttackCoolDown => attackCoolDown;
        public float CoolDownRotationSpeed => coolDownRotationSpeed;

        public float ConfusingSoundDetectionRange => confusingSoundDetectionRange;

        public float DetectionVelocity => detectionVelocity;
        public float AMaxIncrementPerFrame => aMaxIncrementPerFrame;
        public float At => at;
        public float AtMAX => atMAX;
        public float DistanceLowerBound => distanceLowerBound;
        public float AIntensity => aIntensity;
        public float ADecay => aDecay;
        public float DecayGrowthRate => decayGrowthRate;
        public float ARange => aRange;

        public float Ct => ct;
        public float CtMAX => ctMAX;
        public float CIntensity => cIntensity;
        public float CDecayFactor => cDecayFactor;

        public SOSoundSource SoundChaseStatePhrases => soundChaseStatePhrases;
        public SOSoundSource PlayerChaseStatePhrases => playerChaseStatePhrases;
        public SOSoundSource AttackStatePhrases => attackStatePhrases;
        public SOSoundSource CheckSoundStatePhrases => checkSoundStatePhrases;
        public SOSoundSource LostTargetPhrases => lostTargetPhrases;
        public SOSoundSource ConfusedStatePhrases => confusedStatePhrases;


        void OnValidate()
        {
            patrolSpeed = Mathf.Clamp01(patrolSpeed);

            attackDuration = attackAnimation != null ? attackAnimation.length : attackDuration;
            attackDamageDelay = Mathf.Clamp(attackDamageDelay, 0f, attackDuration);
            attackDamageWindowTime = Mathf.Clamp(attackDamageWindowTime, 0f, attackDuration - attackDamageDelay);
            attackCoolDown = Mathf.Max(attackCoolDown, 0f);
            attackRange = Mathf.Clamp(attackRange, 0f, fovData.Range - 0.1f);


        }

    }
}
