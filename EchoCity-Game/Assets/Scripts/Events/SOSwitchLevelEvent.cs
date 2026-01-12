using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SwitchLevelEvent", menuName = "ECHO CITY/Events/Switch Level")]
    public class SOSwitchLevelEvent : SOEventDoubleParam<SceneEnum, EventParams>
    {
        public override void RaiseEvent(IEventSender sender, SceneEnum level, EventParams @params)
        {
            base.RaiseEvent(sender, level, @params);
        }
    }
}
