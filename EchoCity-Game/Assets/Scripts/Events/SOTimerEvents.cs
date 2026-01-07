using UnityEngine;

namespace EchoCity
{
    public enum TimerEventEnum
    {
        NarrationLineEnded
    }

    [CreateAssetMenu(fileName = "TimerEvent", menuName = "ECHO CITY/Events/Timer")]
    public class SOTimerEvent : SOSingleParamEvent<TimerEventEnum>
    {
        public override void RaiseEvent(IEventSender sender, TimerEventEnum timerEvent)
        {
            base.RaiseEvent(sender, timerEvent);
        }
    }
}