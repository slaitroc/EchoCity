using UnityEngine;

namespace EchoCity
{
    public enum TimerEventEnum
    {
        NarrationLineEnded,
        NarrationLineHalfway
    }

    [CreateAssetMenu(fileName = "TimerEvent", menuName = "ECHO CITY/Events/Timer")]
    public class SOTimerEvent : SOEventSingleParam<TimerEventEnum>
    {
        public override void RaiseEvent(IEventSender sender, TimerEventEnum timerEvent)
        {
            base.RaiseEvent(sender, timerEvent);
        }
    }
}