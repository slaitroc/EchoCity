using UnityEngine;
using EchoCity;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private SOPickableData data;
    [SerializeField] private int quantity = 1;

    public SOPickableData Data => data;
    public int Quantity => quantity;

    public InventoryItem(SOPickableData data, int quantity = 1)
    {
        this.data = data;
        this.quantity = quantity;
    }
}
