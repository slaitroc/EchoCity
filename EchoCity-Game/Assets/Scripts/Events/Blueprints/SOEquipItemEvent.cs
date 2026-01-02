using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EquipItemEventSO", menuName = "ECHO CITY/Events/EquipItemEventSO")]
    public class SOEquipItemEvent : SOEventTripleParam<int, SOPickable, GameObject>
    {
        public override void RaiseEvent(IEventSender sender, int index, SOPickable pickableData, GameObject prefab)
        {
            base.RaiseEvent(sender, index, pickableData, prefab);
        }
    }
}