using System;
using System.Collections.Generic;
using UnityEngine;
using EchoCity;

public class PlayerInventory : MonoBehaviour, IEventSender
{

    [SerializeField] private PlayerController playerController;
    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid inventoryChangedEvent;
    [SerializeField] private SOBoolEvent canBePickedEvent;

    string IEventSender.SenderName => gameObject.name;
    int IEventSender.SenderID => GetInstanceID();
    bool IEventSender.IsManager => false;
    EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Player };

    [Header("Observing Events")]
    [SerializeField] private SOIntEvent dropItemEvent;
    [SerializeField] private SOPickableDataGameObjectEvent pickItemEvent;
    [SerializeField] private SOSceneEnumEvent switchToInitStateEvent;

    [Header("Inventory")]
    [SerializeField] private InventoryItem[] itemsArray;
    [SerializeField] private SOPickable handsSoundTool;
    private GameObject[] prefabsArray;
    public IReadOnlyList<InventoryItem> Items => itemsArray;
    public IReadOnlyList<GameObject> Prefabs => prefabsArray;


    void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponentInChildren<PlayerController>();
        }
        Debug.Assert(playerController != null, "PlayerInventory requires a PlayerController reference.");

        itemsArray = new InventoryItem[8];
        prefabsArray = new GameObject[itemsArray.Length];
        if (handsSoundTool != null)
        {
            InventoryItem handsItem = new InventoryItem(new PickableData(handsSoundTool));
            itemsArray[0] = handsItem;
        }
    }

    void OnEnable()
    {
        if (pickItemEvent) pickItemEvent.OnEventRaised += AddItemHandler;
        if (dropItemEvent) dropItemEvent.OnEventRaised += RemoveItemHandler;
        if (switchToInitStateEvent) switchToInitStateEvent.OnEventRaised += Clear;
    }

    void OnDisable()
    {
        if (pickItemEvent) pickItemEvent.OnEventRaised -= AddItemHandler;
        if (dropItemEvent) dropItemEvent.OnEventRaised -= RemoveItemHandler;
        if (switchToInitStateEvent) switchToInitStateEvent.OnEventRaised -= Clear;
    }

    public void AddItemHandler(IEventSender sender, PickableData data, GameObject pickablePrefab)
    {
        // For now it will be non-stacking: each item is a separate entry
        InventoryItem item = new InventoryItem(data);
        bool added = false;
        for (int i = 0; i < itemsArray.Length; i++)
        {
            if (itemsArray[i] == null)
            {
                itemsArray[i] = item;
                prefabsArray[i] = pickablePrefab;
                added = true;
                break;
            }
        }
        if (added) inventoryChangedEvent?.RaiseEvent(this);
        else playerController.EmitFullInventorySound();
        canBePickedEvent?.RaiseEvent(this, added);
    }

    public void RemoveItemHandler(IEventSender sender, int itemIndex)
    {
        prefabsArray[itemIndex] = null;
        itemsArray[itemIndex] = null;
    }

    public void Clear(IEventSender sender, EchoCity.SceneEnum scene)
    {
        itemsArray = new InventoryItem[8];
        prefabsArray = new GameObject[itemsArray.Length];
        if (handsSoundTool != null)
        {
            InventoryItem handsItem = new InventoryItem(new PickableData(handsSoundTool));
            itemsArray[0] = handsItem;
        }
        inventoryChangedEvent?.RaiseEvent(this);
    }
}
