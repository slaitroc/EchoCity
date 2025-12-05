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
    public class PlayerController : MonoBehaviour
    {

        private const string LOG_TAG = "PLAYER CONTROLLER";
        private const string LOG_COLOR = "#39e8d1ff";

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOEventVoid materialToggleEvent;
        [SerializeField] private SOIntEvent itemDroppedEvent;
        [SerializeField] private SOEventVoid deathEvent;

        [Header("Observing Events")]
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;
        [SerializeField] private SOEnemyAIEvent playerHitEvent;

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

        void OnEnable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised += EquipItem;

            if (playerHitEvent)
                playerHitEvent.OnEventRaised += playerHit;
        }

        void OnDisable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised -= EquipItem;

            if (playerHitEvent)
                playerHitEvent.OnEventRaised -= playerHit;
        }

        void Start()
        {
            currentHealth = maxHealth;
            _lastTimeDamaged = float.NegativeInfinity;
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
            ECSound.PlaySoundAtPosition(fullInventorySound, transform.position, newAudioSphereEvent, "SFX");
        }

        public void EquipItem(int index, PickableData data, GameObject prefab)
        {
            Log.D($"Equipping item", LOG_COLOR, LOG_TAG);
            equippedItem = new EquippedItem(index, data, prefab);
            if (prefab == null)
                Log.E($"EquipItem received null prefab for item '{data.Name}' (index {index})", LOG_COLOR, LOG_TAG);

        }

        public void playerHit(EnemyAI enemy)
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
                    ECSound.PlaySoundAtPosition(equippedItem.Data.ToolSound, transform.position, newAudioSphereEvent, "SFX");
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
