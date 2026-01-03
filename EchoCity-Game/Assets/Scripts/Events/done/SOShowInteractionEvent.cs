using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "BoolStringEventSO", menuName = "ECHO CITY/Events/BoolStringEventSO")]
    public class SOShowInteractionEvent : SOEventTripleParam<bool, bool, string>
    {
        public override void RaiseEvent(IEventSender sender, bool isRaycastInteractable, bool showDescription, string description = null)
        {
            base.RaiseEvent(sender, isRaycastInteractable, showDescription, description);
        }
    }
}
