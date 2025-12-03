using System;
using System.Collections.Generic;
using UnityEngine;
using EchoCity;

public class PlayerInventory : MonoBehaviour
{

    private const string LOG_TAG = "PLAYER INVENTORY";
    private const string LOG_COLOR = "#39e8d1ff";
    [SerializeField] private PlayerController playerController;
    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid inventoryChangedEvent;
    [SerializeField] private SOBoolEvent canBePickedEvent;


    [Header("Observing Events")]
    [SerializeField] private SOIntegerGameObjectEvent dropItemEvent;
    [SerializeField] private SOPickableDataGameObjectEvent pickItemEvent;

    [Header("Inventory")]
    private InventoryItem[] itemsArray = new InventoryItem[8];
    private GameObject[] prefabsArray = new GameObject[8];
    public IReadOnlyList<InventoryItem> Items => itemsArray;
    public IReadOnlyList<GameObject> Prefabs => prefabsArray;

    void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponentInChildren<PlayerController>();
        }
        if (playerController == null)
        {
            Log.E("PlayerController not found in PlayerInventory", LOG_COLOR, LOG_TAG);
        }
    }

    void OnEnable()
    {
        if (pickItemEvent) pickItemEvent.OnEventRaised += AddItemHandler;
    }

    void OnDisable()
    {
        if (pickItemEvent) pickItemEvent.OnEventRaised -= AddItemHandler;
    }


    public void AddItemHandler(PickableData data, GameObject pickablePrefab)
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
        if (added) inventoryChangedEvent?.RaiseEvent();
        else playerController.EmitFullInventorySound();
        canBePickedEvent?.RaiseEvent(added);
    }

    public void RemoveItemHandler(int itemIndex)
    {
        prefabsArray[itemIndex] = null;
        itemsArray[itemIndex] = null;
        inventoryChangedEvent?.RaiseEvent();
        //playerController.DropItem(index, prefabsArray[index]);

    }

    public void Clear()
    {
        for (int i = 0; i < itemsArray.Length; i++)
        {
            itemsArray[i] = null;
            prefabsArray[i] = null;
        }

        inventoryChangedEvent?.RaiseEvent();
    }
}
