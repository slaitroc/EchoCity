using UnityEngine;
using UnityEngine.UI;

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
    public class PlayerController : MonoBehaviour, IDamageable
    {

        private const string LOG_TAG = "PLAYER CONTROLLER";
        private const string LOG_COLOR = "#39e8d1ff";

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOEventVoid materialToggleEvent;
        [SerializeField] private SOIntEvent itemDroppedEvent;
        [SerializeField] private SOEventVoid deathEvent;
        [SerializeField] private SOSoundEmissionDataVector3 playerEmittedSoundEvent;


        [Header("Observing Events")]
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;

        [Header("Inventory")]
        public SOSoundSource fullInventorySound;
        public EquippedItem equippedItem = null;
        [SerializeField] private Transform dropPoint;

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

        void OnEnable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised += EquipItemHandler;
        }

        void OnDisable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised -= EquipItemHandler;
        }

        void Start()
        {
            _audioContext = new AudioContext(newAudioSphereEvent);

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

        public void EquipItemHandler(int index, PickableData data, GameObject prefab)
        {
            Log.D($"Equipping item", LOG_COLOR, LOG_TAG);
            equippedItem = new EquippedItem(index, data, prefab);
            if (prefab == null)
                Log.E($"EquipItem received null prefab for item '{data.Name}' (index {index})", LOG_COLOR, LOG_TAG);

        }

        public void TakeDamage(float damageAmount)
        {
            Log.D($"Taking {damageAmount} damage.", LOG_COLOR, LOG_TAG);
            currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
            _lastTimeDamaged = Time.time;
            if (currentHealth <= 0)
            {
                Log.W("YOU DIED", "red", LOG_TAG);
                deathEvent?.RaiseEvent();
            }
        }

        public void UseTool()
        {
            if (equippedItem != null)
            {
                if (equippedItem.Data.PickableType == PickableType.SoundTool)
                {
                    EchoCitySound.PlayRandomInAudioSource(equippedItem.Data.ToolSound, _audioContext, _playerAudioSource, EchoCitySound.MixerGroupEnum.SFX);
                    playerEmittedSoundEvent?.RaiseEvent(transform.position, new SoundEmissionData(transform.position, equippedItem.Data.ToolSound));
                    return;
                }
                else if (equippedItem.Data.PickableType == PickableType.Tool)
                {
                    //use tool only when pointing interacting objects
                }
            }
            else
            {
                Log.D("No item equipped in the specified slot.", "#39e8d1ff", "PLAYER CONTROLLER");
            }
        }
        public void DropItem()
        {
            if (equippedItem == null) return;
            if (equippedItem.Index == 0) return;

            Vector3 dropPosition = dropPoint != null ? dropPoint.position : transform.position + transform.forward;
            if (equippedItem.Prefab == null)
            {
                Log.E("Tried to drop an item but equipped prefab is null. Drop cancelled.", "#ff6666ff", "PLAYER CONTROLLER");
            }
            else
            {
                Instantiate(equippedItem.Prefab, dropPosition, Quaternion.identity);
            }
            materialToggleEvent?.RaiseEvent();
            materialToggleEvent?.RaiseEvent();
            itemDroppedEvent?.RaiseEvent(equippedItem.Index);
            equippedItem = null;
        }
    }
}
