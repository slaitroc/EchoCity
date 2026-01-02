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
        [SerializeField] private SOSceneEnumEvent switchToInitStateEvent;

        [Header("Inventory")]
        [SerializeField] private InventoryItem[] itemsArray;
        [SerializeField] private SOPickable handsSoundTool;
        private GameObject[] prefabsArray;
        public IReadOnlyList<InventoryItem> Items => itemsArray;
        public IReadOnlyList<GameObject> Prefabs => prefabsArray;

        void Awake()
        {
            itemsArray = new InventoryItem[8];
            prefabsArray = new GameObject[itemsArray.Length];
            if (handsSoundTool != null)
                itemsArray[0] = new InventoryItem(handsSoundTool);
        }

        void OnEnable()
        {
            if (switchToInitStateEvent) switchToInitStateEvent.OnEventRaised += ClearHandler;
        }

        void OnDisable()
        {
            if (switchToInitStateEvent) switchToInitStateEvent.OnEventRaised -= ClearHandler;
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
            prefabsArray = new GameObject[itemsArray.Length];
            if (handsSoundTool != null)
                itemsArray[0] = new InventoryItem(handsSoundTool);
            inventoryChangedEvent?.RaiseEvent(this, PickablesEnum.None, PickableTypeEnum.None, InventoryCodesEnum.Cleared);
        }

        public void ClearHandler(IEventSender sender, EchoCity.SceneEnum scene) => Clear();

    }
}
