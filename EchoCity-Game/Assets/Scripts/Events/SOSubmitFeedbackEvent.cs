using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SubmitFeedbackEvent", menuName = "ECHO CITY/Events/Submit Feedback")]
    public class SOSubmitFeedbackEvent : SOEventDoubleParam<int, string>
    {
        public override void RaiseEvent(IEventSender sender, int rating, string comments)
        {
            base.RaiseEvent(sender, rating, comments);
        }
    }
}
