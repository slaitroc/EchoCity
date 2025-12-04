using UnityEngine;

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

        [Header("Observing Events")]
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;


        [Header("Inventory")]
        public SOSoundSource fullInventorySound;
        public EquippedItem equippedItem = null;
        [SerializeField] private Transform dropPoint;

        void OnEnable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised += EquipItem;
        }

        void OnDisable()
        {
            if (itemEquippedEvent)
                itemEquippedEvent.OnEventRaised -= EquipItem;
        }

        public void EmitFullInventorySound()
        {
            ECSound.PlaySoundAtPosition(fullInventorySound, transform.position, newAudioSphereEvent, "SFX");
        }

        public void EquipItem(int index, PickableData data, GameObject prefab)
        {
            equippedItem = new EquippedItem(index, data, prefab);
            if (prefab == null)
                Log.E($"EquipItem received null prefab for item '{data.Name}' (index {index})", LOG_COLOR, LOG_TAG);

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