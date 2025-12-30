using UnityEngine;
using UnityEngine.AI;

namespace EchoCity
{
    [System.Serializable]
    public struct PatrolArea
    {
        [SerializeField] private string name;
        [SerializeField] private Transform[] waypoints;

        public string Name => name;
        public Transform[] Waypoints => waypoints;

        public Vector3 GetCenter()
        {
            if (Waypoints == null || Waypoints.Length == 0) return Vector3.zero;

            Vector3 center = Vector3.zero;
            int validWaypoints = 0;

            foreach (var waypoint in Waypoints)
            {
                if (waypoint != null)
                {
                    center += waypoint.position;
                    validWaypoints++;
                }
            }
            return validWaypoints > 0 ? center / validWaypoints : Vector3.zero;
        }

        public PatrolArea(string name, Transform[] waypoints)
        {
            this.name = name;
            this.waypoints = waypoints;
        }
    }

    [System.Serializable]
    public class PerceivedSound
    {
        [SerializeField] private float _initTime;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Frequency _frequency;
        [SerializeField] private float _duration;
        [SerializeField] private float _rangeFactor;
        [SerializeField] private float _intensityFactor;
        [SerializeField] private float _decay;
        [SerializeField] private float _persistence;
        [SerializeField] private float _rangeDecay;
        [SerializeField] private bool _isConfusing;
        [SerializeField] private bool _isEnemy;
        public float InitTime => _initTime;
        public float RemainingTime => (InitTime + Duration) - Time.time;
        public Vector3 Position => _position;
        public Frequency Frequency => _frequency;
        public float Duration => _duration;
        public float RangeFactor => _rangeFactor;
        public float IntensityFactor => _intensityFactor;
        public float Decay => _decay;
        public float Persistence => _persistence;
        public bool IsConfusing => _isConfusing;
        public bool IsEnemy => _isEnemy;

        public PerceivedSound(float time)
        {
            _initTime = time;
            _position = Vector3.zero;
            _frequency = Frequency.Low;
            _duration = 0f;
            _rangeFactor = 0f;
            _intensityFactor = 0f;
            _decay = 0f;
            _persistence = 0.01f;
            _isConfusing = false;
            _isEnemy = false;
        }

        public void UpdatePerceivedSound(SoundEmissionData sound)
        {
            _initTime = Time.time;
            _position = sound.Position;
            _frequency = sound.SoundClass.Frequency;
            _duration = sound.Duration;
            _rangeFactor = sound.SoundClass.RangeFactor;
            _intensityFactor = sound.SoundClass.IntensityFactor * sound.SoundClassIntensityFactorMultiplier;
            _decay = sound.SoundClass.Decay;
            _persistence = sound.SoundClass.Persistence;
            _isConfusing = sound.SoundClass.IsConfusing;
            _isEnemy = sound.SoundClass.IsEnemy;
        }

        public void UpdatePerceivedSound(PerceivedSound other)
        {
            _initTime = other.InitTime;
            _position = other.Position;
            _frequency = other.Frequency;
            _duration = other.Duration;
            _rangeFactor = other.RangeFactor;
            _intensityFactor = other.IntensityFactor;
            _decay = other.Decay;
            _persistence = other.Persistence;
            _isConfusing = other.IsConfusing;
            _isEnemy = other.IsEnemy;
        }

        public void UpdatePosition(Vector3 newPosition) => _position = newPosition;
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class EnemyAI : MonoBehaviour, IFSMOwner, IEnemyContext, IDamageDealer, IHasFOV, ISoundPerceiver, IAttractionSystem, IConfusionSystem
    {
        #region fields and properties
        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOIAttractionEvent enemyAttractionEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Enemy };

        [Header("Observed Events")]
        [SerializeField] private SOSoundEmissionDataEvent enemyPerceivedSoundEvent;

        [Header("Enemy Configuration")]
        [SerializeField] private SOEnemyData enemyData;
        [SerializeField] private EnemyFOV fov;
        [SerializeField] private CollisionHitDetector hitDetector;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;

        [Header("Runtime")]
        [SerializeField] private EnemyStatesEnum currentState;
        [SerializeField] private PatrolArea currentPatrolArea;
        [SerializeField] private PatrolArea[] patrolAreas;
        [SerializeField] private PerceivedSound lastPS;
        [SerializeField] private PerceivedSound targetSound;
        [SerializeField] private PerceivedSound lastAPS; //Attraction Perceived Sound
        [SerializeField] private PerceivedSound lastCPS; //Confusing Perceived Sound

        // IFSMOwner
        private EnemyFSM _fsm;

        public Transform Transform => this.transform;
        public GameObject GameObject => this.gameObject;

        // ENEMY CONTEXT
        public IFSMOwner Owner => this;
        public IFOV FOV => fov;
        public EnemyStatesEnum CurrentStateEnum { get => currentState; set => currentState = value; }
        public SOEnemyData EnemyData => enemyData;
        public NavMeshAgent Agent => agent;
        public PatrolArea[] PatrolAreas => patrolAreas;
        public PatrolArea CurrentPatrolArea { get => currentPatrolArea; set => currentPatrolArea = value; }
        public Animator Animator => animator;
        public AudioSource AudioSource => audioSource;
        public IHitDetector HitDetector => hitDetector;
        public PerceivedSound LastPerceivedSound => lastPS;
        public PerceivedSound TargetSound => targetSound;
        public IAttractionSystem AttractionSystem => this;
        public IConfusionSystem ConfusionSystem => this;
        public SOSoundEmissionDataEvent NewAudioSphereEvent => newAudioSphereEvent;
        public SOIAttractionEvent EnemyAttractionEvent => enemyAttractionEvent;

        // ATTRACTION System
        [SerializeField] private float _A = 0f; //attraction
        private bool _attractionCompute = true;

        PerceivedSound IAttractionSystem.LastPerceivedSound { get => lastAPS; set => lastAPS = value; }
        bool IAttractionSystem.Compute { get => _attractionCompute; set => _attractionCompute = value; }
        public float CurrentAttraction => _A;
        void IAttractionSystem.SetAttraction(float value) => _A = value;

        // CONFUSION System
        [SerializeField] private float _C = 0f; //confusion
        private bool _confusionCompute = true;

        PerceivedSound IConfusionSystem.LastPerceivedSound { get => lastCPS; set => lastCPS = value; }
        bool IConfusionSystem.Compute { get => _confusionCompute; set => _confusionCompute = value; }
        public float CurrentConfusion => _C;

        void IConfusionSystem.SetConfusion(float value) => _C = value;
        #endregion

        void Awake()
        {
            Debug.Assert(TryGetComponent(out animator), "EnemyAI requires an Animator component.", this);
            Debug.Assert(TryGetComponent(out audioSource), "EnemyAI requires an AudioSource component.", this);
            Debug.Assert(TryGetComponent(out agent), "EnemyAI requires a NavMeshAgent component.", this);
            Debug.Assert(fov != null, "EnemyAI requires a FOV component.", this);
            Debug.Assert(enemyData != null, "No SOEnemyData assigned to EnemyAI on " + gameObject.name, this);
            Debug.Assert(hitDetector != null, "No AttackRangeDetector assigned to EnemyAI on " + gameObject.name, this);
        }

        void OnEnable()
        {
            if (enemyPerceivedSoundEvent != null) enemyPerceivedSoundEvent.OnEventRaised += PerceivedSoundHandler;
        }

        void OnDisable()
        {
            if (enemyPerceivedSoundEvent != null) enemyPerceivedSoundEvent.OnEventRaised -= PerceivedSoundHandler;
        }

        void Start()
        {
            InitializeFOV(enemyData.FOVData); // Initialize FOV with enemy data
            InitPerceivedSounds();
            FindPatrolAreas();
            _fsm = new EnemyFSM(this);
            _fsm.Initialize();
        }

        void Update()
        {
            UpdateFOV();
            AttractionComputation();
            ConfusionComputation();
            _fsm.Update();
            NotifyUI();
            //agent.isStopped = true; // DEBUG: stop movement for testing
        }

        //<summary> Finds and assigns patrol areas in the scene </summary>
        private void FindPatrolAreas()
        {
            var areas = GameObject.FindGameObjectsWithTag("PatrolArea");
            patrolAreas = new PatrolArea[areas.Length];
            for (int i = 0; i < areas.Length; i++)
            {
                var areaObj = areas[i];
                var waypoints = new Transform[areaObj.transform.childCount];
                for (int j = 0; j < areaObj.transform.childCount; j++)
                {
                    waypoints[j] = areaObj.transform.GetChild(j);
                }
                patrolAreas[i] = new PatrolArea(areaObj.name, waypoints);
            }
        }

        private void InitPerceivedSounds()
        {
            if (lastPS == null)
                lastPS = new PerceivedSound(Time.time);
            if (targetSound == null)
                targetSound = new PerceivedSound(Time.time);
            if (lastAPS == null)
                lastAPS = new PerceivedSound(Time.time);
            if (lastCPS == null)
                lastCPS = new PerceivedSound(Time.time);
        }

        public void PerceivedSoundHandler(IEventSender sender, SoundEmissionData sound)
        {
            if (sound.SoundClass.IsEnemy == true) return; // ignore enemy sounds
            //NOTE environmental sound could become the source of confusion
            if (sound.IsEnvironmental == true) return; // ignore environmental sounds
            if (sound.SoundClass.IsPlayerBodySound == true)
            {
                if (lastPS.RemainingTime <= 0f)
                {
                    lastPS.UpdatePerceivedSound(sound);
                }
            }
            else
                lastPS.UpdatePerceivedSound(sound);
        }
        public void DealDamage(IDamageable damageable) => _fsm.CurrentState.DealDamage(damageable);

        // calculate attraction based on lastPS
        public void AttractionComputation()
        {
            if (_attractionCompute == true)
            {
                // phase 1: increase A if there is an active sound 
                if (lastPS.RemainingTime > 0f && _A < enemyData.AtMAX)
                {
                    float dist = Vector3.Distance(lastPS.Position, transform.position);
                    dist = Mathf.Max(dist, enemyData.DistanceLowerBound);

                    // distance based attenuation
                    // 1.0f at 0 distance, decreases with distance 
                    float distanceAttenuation = 1f / (1f + Mathf.Pow(dist / (lastPS.RangeFactor * enemyData.ARange), lastPS.Decay));

                    // increment based on specific sound intensity and global enemy intensity
                    float tempA = enemyData.AIntensity * lastPS.IntensityFactor * distanceAttenuation * Time.deltaTime;

                    // Clamp per frame
                    _A += Mathf.Min(tempA, enemyData.AMaxIncrementPerFrame);
                }

                // phase 2: decrease A (always active or post-sound)
                // If there are no active sounds, increase the "silence" timer
                var _timeSinceLastSound = 0f;
                if (lastPS.RemainingTime <= 0f)
                    _timeSinceLastSound += Time.deltaTime;

                if (_A > 0f)
                {
                    // Decay accelerates over time: the longer since the sound, the faster A decreases
                    // We use quadratic or exponential growth for decay acceleration
                    float decayAcceleration = 1f + (_timeSinceLastSound * enemyData.DecayGrowthRate);
                    float drop = enemyData.ADecay * (1f / lastPS.Persistence) * decayAcceleration * Time.deltaTime;

                    _A -= drop;
                    if (_A < 0f) _A = 0f;
                }
            }
            if (_A >= enemyData.At && lastPS != lastAPS)
                lastAPS.UpdatePerceivedSound(lastPS);
        }

        // calculate confusion based on lastPS (isConfusing)
        public void ConfusionComputation()
        {
            if (_confusionCompute == true && lastPS.IsConfusing)
            {
                if (lastCPS.Position != Vector3.zero && lastCPS.RemainingTime <= 0f)
                    if (_C > 0f)
                        _C -= lastCPS.Decay * enemyData.CDecayFactor * Time.deltaTime;
                    else
                        _C = 0f;
                else
                {
                    var distance = Vector3.Distance(lastPS.Position, transform.position);

                    _C += enemyData.CIntensity
                                    * lastPS.IntensityFactor
                                    / Mathf.Pow((distance + 0.01f) * lastPS.RangeFactor, lastPS.Decay)
                                    * Time.deltaTime;
                }
                if (_C >= enemyData.Ct && lastPS != lastCPS && lastPS.IsConfusing)
                    lastCPS.UpdatePerceivedSound(lastPS);
            }
        }

        /// <summary>
        /// Notifies UI with current attraction value (UI developer handles all thresholds and logic)
        /// </summary>
        void NotifyUI() { }

        #region FOV Methods
        public void InitializeFOV(FOVParams fovData)
        {
            if (fov == null) return;
            fov.Initialize(fovData, transform);
        }

        public void UpdateFOV()
        {
            if (fov == null) return;
            fov.UpdateTargets();
        }
        #endregion

        void OnDrawGizmos()
        {
            //attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
        }
    }
}

