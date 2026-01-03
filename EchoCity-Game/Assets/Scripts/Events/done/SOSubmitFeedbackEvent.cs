using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "IntStringEventSO", menuName = "ECHO CITY/Events/IntStringEventSO")]
    public class SOSubmitFeedbackEvent : SOEventDoubleParam<int, string>
    {
        public override void RaiseEvent(IEventSender sender, int rating, string comments)
        {
            base.RaiseEvent(sender, rating, comments);
        }
    }
}
