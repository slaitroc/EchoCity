using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SOInventoryChangedEvent", menuName = "ECHO CITY/Events/SOInventoryChangedEvent")]
    public class SOInventoryChangedEvent : SOEventTripleParam<PickablesEnum, PickableTypeEnum, InventoryCodesEnum>
    {
        public override void RaiseEvent(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            base.RaiseEvent(sender, pickable, pickableType, inventoryCodes);
        }
    }
}