using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "ItemUsedEvent", menuName = "ECHO CITY/Events/Item Used")]
    public class SOItemUsedEvent : SOEventSingleParam<SOPickable>
    {
        public override void RaiseEvent(IEventSender sender, SOPickable toolUsed)
        {
            base.RaiseEvent(sender, toolUsed);
        }
    }
}