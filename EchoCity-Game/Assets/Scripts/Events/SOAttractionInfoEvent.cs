using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "AttractionInfoEvent", menuName = "ECHO CITY/Events/Attraction Info")]
    public class SOAttractionInfoEvent : SOEventTripleParam<IAttraction, Transform, bool>
    {
        public override void RaiseEvent(IEventSender sender, IAttraction attraction, Transform targetTransform, bool aboveThreshold)
        {
            base.RaiseEvent(sender, attraction, targetTransform, aboveThreshold);
        }
    }
}