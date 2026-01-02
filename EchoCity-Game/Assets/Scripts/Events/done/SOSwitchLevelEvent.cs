using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SwitchLevelEventSO", menuName = "ECHO CITY/Events/SwitchLevelEventSO")]
    public class SOSwitchLevelEvent : SOEventDoubleParam<SceneEnum, EventParams>
    {
        public override void RaiseEvent(IEventSender sender, SceneEnum level, EventParams @params)
        {
            base.RaiseEvent(sender, level, @params);
        }
    }

    public class ToLevelParams : EventParams
    {
        //level difficulty
    }
}
