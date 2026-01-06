using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EquippedItemChangedEvent", menuName = "ECHO CITY/Events/Equipped Item Changed")]
    public class SOEquippedItemChangedEvent : SOSingleParamEvent<PickablesEnum>
    {
        public override void RaiseEvent(IEventSender sender, PickablesEnum newEquippedItem)
        {
            base.RaiseEvent(sender, newEquippedItem);
        }
    }
}