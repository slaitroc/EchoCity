using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EchoMaterialUpdatedEvent", menuName = "ECHO CITY/Events/Echo Material Updated")]
    public class SOEchoMaterialUpdated : SOEventSingleParam<bool>
    {
        public override void RaiseEvent(IEventSender sender, bool value)
        {
            base.RaiseEvent(sender, value);
        }
    }
}