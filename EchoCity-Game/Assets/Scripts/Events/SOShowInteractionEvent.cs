using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "ShowInteractionEvent", menuName = "ECHO CITY/Events/Show Interaction")]
    public class SOShowInteractionEvent : SOEventTripleParam<bool, bool, string>
    {
        public override void RaiseEvent(IEventSender sender, bool isRaycastInteractable, bool showDescription, string description = null)
        {
            base.RaiseEvent(sender, isRaycastInteractable, showDescription, description);
        }
    }
}

