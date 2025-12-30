using UnityEngine;
using UnityEngine.UI;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    [System.Serializable]
    public class EquippedItem
    {
        public int Index;
        public PickableData Data;
        public GameObject Prefab;

        public EquippedItem(int index, PickableData data, GameObject prefab)
        {
            Index = index;
            Data = data;
            Prefab = prefab;
        }
    }

    public class AttractionTarget
    {
        public Transform Transform;
        public IAttraction AttractionData;

        public AttractionTarget(Transform transform, IAttraction data)
        {
            Transform = transform;
            AttractionData = data;
        }
    }

    public class PlayerController : MonoBehaviour, IDamageable, ISoundPerceiver, IAttractionSystem, IEventSender
    {

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOEventVoid materialToggleEvent;
        [SerializeField] private SOIntEvent itemDroppedEvent;
        [SerializeField] private SOEventVoid deathEvent;
        [SerializeField] private SOSoundEmissionDataVector3 playerEmittedSoundEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Player };

        [Header("Observing Events")]
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;
        [SerializeField] private SOIAttractionEvent enemyAttractionEvent;
        [SerializeField] private SOSoundEmissionDataEvent perceivedSoundEvent;


        [Header("Inventory")]
        public SOSoundSource fullInventorySound;
        public EquippedItem equippedItem = null;
        [SerializeField] private Transform dropPoint;

        [Header("Attraction")]
        [SerializeField] private float fixedDistanceEstimation = 7f;
        [SerializeField] private SOEnemyData sampleEnemy;
        [SerializeField] private float activeAttraction; //DEBUG
        private AttractionTarget attractionTarget;
        private AttractionTarget[] _attractionTargets;
        private float _A = 0f;
        private PerceivedSound lastPS;
        private bool _attractionCompute = true;

        [Header("Health Settings")]
        public float maxHealth = 100f;
        public float damageAmount = 70f;
        public float healthRegenRate = 10f;
        public float healthRegenDelay = 2f;

        public float currentHealth = 100f;
        public Image overlayImage;

        private float _lastTimeDamaged;

        private AudioContext _audioContext;

        [Header("Audio")]
        private GameObject _playerToolsAudio;
        private AudioSource _playerAudioSource;

        public bool Compute { get => _attractionCompute; set => _attractionCompute = value; }
        public float CurrentAttraction => attractionTarget != null ? attractionTarget.AttractionData.CurrentAttraction : _A;
        public PerceivedSound LastPerceivedSound { get => lastPS; set => lastPS = value; }


        void OnEnable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised += EquipItemHandler;
            if (enemyAttractionEvent != null)
                enemyAttractionEvent.OnEventRaised += UpdateActiveAttractionTargets;
            if (perceivedSoundEvent != null)
                perceivedSoundEvent.OnEventRaised += PerceivedSoundHandler;
        }

        void OnDisable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised -= EquipItemHandler;
            if (enemyAttractionEvent != null)
                enemyAttractionEvent.OnEventRaised -= UpdateActiveAttractionTargets;
            if (perceivedSoundEvent != null)
                perceivedSoundEvent.OnEventRaised -= PerceivedSoundHandler;
        }


        void Awake()
        {
            _attractionTargets = new AttractionTarget[3];
            lastPS = new PerceivedSound(0f);
        }
        void Start()
        {
            _audioContext = new AudioContext(this, newAudioSphereEvent);

            currentHealth = maxHealth;
            _lastTimeDamaged = float.NegativeInfinity;

            _playerToolsAudio = new GameObject("ToolsAudioSource");
            _playerToolsAudio.transform.SetParent(transform);
            _playerToolsAudio.transform.localPosition = Vector3.zero;
            _playerAudioSource = _playerToolsAudio.AddComponent<AudioSource>();
            _playerAudioSource.spatialBlend = 1.0f; // 3D sound
        }


        void Update()
        {
            activeAttraction = CurrentAttraction; //DEBUG
            UpdateAttractionTarget();
            AttractionComputation();

            // Health regeneration over time
            if (currentHealth < maxHealth && currentHealth > 0 && Time.time - _lastTimeDamaged > healthRegenDelay)
            {
                currentHealth += healthRegenRate * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            }

            // Update overlay opacity based on health
            if (overlayImage != null)
            {
                float healthPercentage = currentHealth / maxHealth;
                Color overlayColor = overlayImage.color;
                overlayColor.a = 1f - Mathf.Pow(healthPercentage, 2);
                overlayImage.color = overlayColor;
            }
        }


        public void EmitFullInventorySound()
        {
            EchoCitySound.PlayInAudioSource(fullInventorySound.AudioClip, fullInventorySound.Volume, _playerAudioSource, EchoCitySound.MixerGroupEnum.SFX);
        }

        public void EquipItemHandler(IEventSender sender, int index, PickableData data, GameObject prefab)
        {
            Log.DLazy(() => "Equipping item", this);
            equippedItem = new EquippedItem(index, data, prefab);
            if (prefab == null)
                Log.ELazy(() => $"EquipItem received null prefab for item '{data.Name}' (index {index})", this);

        }

        public void TakeDamage(float damageAmount)
        {
            Log.DLazy(() => $"Taking {damageAmount} damage.", this);
            currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
            _lastTimeDamaged = Time.time;
            if (currentHealth <= 0)
            {
                Log.W("YOU DIED", "-", "red");
                deathEvent?.RaiseEvent(this);
            }
        }

        public void UseTool()
        {
            if (equippedItem != null)
            {
                if (equippedItem.Data.PickableType == PickableType.SoundTool)
                {
                    PlayRandomInAudioSource(equippedItem.Data.ToolSound, _audioContext, _playerAudioSource, MixerGroupEnum.SFX);
                    playerEmittedSoundEvent?.RaiseEvent(this, transform.position, new SoundEmissionData(transform.position, equippedItem.Data.ToolSound));
                    return;
                }
                else if (equippedItem.Data.PickableType == PickableType.Tool)
                {
                    //use tool only when pointing interacting objects
                }
            }
            else
            {
                Log.DLazy(() => "No item equipped in the specified slot.", this);
            }
        }

        public void DropItem()
        {
            if (equippedItem == null) return;
            if (equippedItem.Index == 0) return;

            Vector3 dropPosition = dropPoint != null ? dropPoint.position : transform.position + transform.forward;
            if (equippedItem.Prefab == null)
            {
                Log.ELazy(() => "Tried to drop an item but equipped prefab is null. Drop cancelled.", this);
            }
            else
            {
                Instantiate(equippedItem.Prefab, dropPosition, Quaternion.identity);
            }
            materialToggleEvent?.RaiseEvent(this);
            materialToggleEvent?.RaiseEvent(this);
            itemDroppedEvent?.RaiseEvent(this, equippedItem.Index);
            equippedItem = null;
        }

        // player estimated attraction computation
        public void AttractionComputation()
        {
            if (_attractionCompute == true)
            {
                // phase 1: increase A if there is an active sound 
                if (lastPS.RemainingTime > 0f && _A < sampleEnemy.AtMAX)
                {
                    //NOTE: using fixed distance estimation for player-enemy distance
                    float dist = fixedDistanceEstimation;
                    dist = Mathf.Max(dist, sampleEnemy.DistanceLowerBound);

                    // distance based attenuation
                    // 1.0f at 0 distance, decreases with distance 
                    float distanceAttenuation = 1f / (1f + Mathf.Pow(dist / (lastPS.RangeFactor * sampleEnemy.ARange), lastPS.Decay));

                    // increment based on specific sound intensity and global enemy intensity
                    float tempA = sampleEnemy.AIntensity * lastPS.IntensityFactor * distanceAttenuation * Time.deltaTime;

                    // Clamp per frame
                    _A += Mathf.Min(tempA, sampleEnemy.AMaxIncrementPerFrame);
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
                    float decayAcceleration = 1f + (_timeSinceLastSound * sampleEnemy.DecayGrowthRate);
                    float drop = sampleEnemy.ADecay * (1f / lastPS.Persistence) * decayAcceleration * Time.deltaTime;

                    _A -= drop;
                    if (_A < 0f) _A = 0f;
                }
            }
        }

        public void SetAttraction(float value)
        {
            _A = Mathf.Clamp(value, 0f, sampleEnemy.AtMAX);
        }

        public void PerceivedSoundHandler(IEventSender sender, SoundEmissionData sound)
        {
            if (sound.SoundClass.IsEnemy == true) return; // ignore enemy sounds
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

        private void UpdateActiveAttractionTargets(IEventSender sender, IAttraction attraction, Transform transform, bool isAboveThreshold)
        {
            if (isAboveThreshold)
            {
                for (int i = 0; i < _attractionTargets.Length - 1; i++)
                {
                    if (_attractionTargets[i] == null)
                    {
                        _attractionTargets[i] = new AttractionTarget(transform, attraction);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _attractionTargets.Length; i++)
                {
                    if (_attractionTargets[i].Transform == transform)
                    {
                        _attractionTargets[i] = null;
                        break;
                    }
                }
            }
        }

        private void UpdateAttractionTarget()
        {
            if (_attractionTargets.Length == 0)
            {
                attractionTarget = null;
                return;
            }

            AttractionTarget closest = null;
            float closestDist = float.MaxValue;

            foreach (AttractionTarget t in _attractionTargets)
            {
                if (t != null)
                {
                    float dist = Vector3.Distance(transform.position, t.Transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = t;
                    }
                }
            }
            attractionTarget = closest;
        }
    }
}
