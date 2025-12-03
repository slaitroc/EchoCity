using UnityEngine;
using EchoCity;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private PickableData data;

    public PickableData Data => data;

    public InventoryItem(PickableData data)
    {
        this.data = data;
    }

}
