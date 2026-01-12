using System.Collections.Generic;
using UnityEngine;

namespace EchoCity
{
    public enum InventoryCodesEnum
    {
        ItemAdded,
        ItemDropped,
        Cleared
    }
    public class PlayerInventory : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => false;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Player };

        [Header("Observing Events")]
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;

        [Header("Inventory")]
        [SerializeField] private InventoryItem[] itemsArray;
        [SerializeField] private SOPickable handsSoundTool;
        private GameObject[] prefabsArray;
        public IReadOnlyList<InventoryItem> Items => itemsArray;
        public IReadOnlyList<GameObject> Prefabs => prefabsArray;

        void Awake()
        {
            itemsArray = new InventoryItem[8];
            for (int i = 0; i < itemsArray.Length; i++)
                itemsArray[i] = null;
            prefabsArray = new GameObject[itemsArray.Length];
            for (int i = 0; i < prefabsArray.Length; i++)
                prefabsArray[i] = null;
            if (handsSoundTool != null)
                itemsArray[0] = new InventoryItem(handsSoundTool);
        }

        void OnEnable()
        {
            // if (switchLevelEvent) switchLevelEvent.OnEventRaised += ClearHandler;
        }

        void OnDisable()
        {
            // if (switchLevelEvent) switchLevelEvent.OnEventRaised -= ClearHandler;
        }

        public bool AddItem(SOPickable data, GameObject pickablePrefab)
        {
            // For now it will be non-stacking: each item is a separate entry
            SOPickable item = data;
            bool added = false;
            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (itemsArray[i] == null)
                {
                    itemsArray[i] = new InventoryItem(item);
                    prefabsArray[i] = pickablePrefab;
                    added = true;
                    break;
                }
            }
            if (added) inventoryChangedEvent?.RaiseEvent(this, item.PickableEnum, item.PickableType, InventoryCodesEnum.ItemAdded);
            return added;
        }

        public bool TryAddItem()
        {
            // For now it will be non-stacking: each item is a separate entry
            bool added = false;
            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (itemsArray[i] == null)
                {
                    added = true;
                    break;
                }
            }
            return added;
        }

        public void DropItem(int itemIndex)
        {
            var item = itemsArray[itemIndex];
            prefabsArray[itemIndex] = null;
            itemsArray[itemIndex] = null;
            inventoryChangedEvent?.RaiseEvent(this, item.Data.PickableEnum, item.Data.PickableType, InventoryCodesEnum.ItemDropped);
        }

        public void Clear()
        {
            itemsArray = new InventoryItem[8];
            for (int i = 0; i < itemsArray.Length; i++)
                itemsArray[i] = null;
            prefabsArray = new GameObject[itemsArray.Length];
            for (int i = 0; i < prefabsArray.Length; i++)
                prefabsArray[i] = null;
            if (handsSoundTool != null)
                itemsArray[0] = new InventoryItem(handsSoundTool);
            inventoryChangedEvent?.RaiseEvent(this, PickablesEnum.None, PickableTypeEnum.None, InventoryCodesEnum.Cleared);
        }

        // public void ClearHandler(IEventSender sender, SceneEnum scene, EventParams @params) => Clear();

    }
}
