using System;
using System.Collections.Generic;
using UnityEngine;
using EchoCity;

public class PlayerInventory : MonoBehaviour
{
    public event Action InventoryChanged;

    [SerializeField] private List<InventoryItem> items = new();

    public IReadOnlyList<InventoryItem> Items => items;

    public void AddItem(SOPickableData data, int quantity = 1)
    {
        if (data == null)
        {
            Debug.LogWarning("PlayerInventory: tried to add null SOPickableData.");
            return;
        }

        // For now it will be non-stacking: each item is a separate entry
        InventoryItem item = new InventoryItem(data, quantity);
        items.Add(item);

        InventoryChanged?.Invoke();
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null) return;

        if (items.Remove(item))
        {
            InventoryChanged?.Invoke();
        }
    }

    public void Clear()
    {
        items.Clear();
        InventoryChanged?.Invoke();
    }
}
