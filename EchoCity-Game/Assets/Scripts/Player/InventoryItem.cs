using UnityEngine;
namespace EchoCity
{
    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private readonly SOPickable _data;
        public SOPickable Data => _data;

        public InventoryItem(SOPickable data)
        {
            _data = data;
        }
    }
}
