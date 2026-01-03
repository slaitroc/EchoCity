using UnityEngine;

namespace EchoCity
{
    public enum InputEnum
    {
        Player,
        UI,
    }
    [CreateAssetMenu(fileName = "PlayerInputEvent", menuName = "ECHO CITY/Events/Player Input")]
    public class SOPlayerInputEvent : SOEventDoubleParam<InputEnum, bool>
    {
        public override void RaiseEvent(IEventSender sender, InputEnum inputType, bool activate)
        {
            base.RaiseEvent(sender, inputType, activate);
        }
    }
}