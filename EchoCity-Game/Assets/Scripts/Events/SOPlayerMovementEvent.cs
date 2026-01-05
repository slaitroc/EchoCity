using UnityEngine;

namespace EchoCity
{
    public enum MovementCodeEnum
    {
        Move,
        Look,
        Sprint,
        Jump,

    }
    [CreateAssetMenu(fileName = "PlayerMovementEvent", menuName = "ECHO CITY/Events/Player Movement")]
    public class SOPlayerMovementEvent : SOSingleParamEvent<MovementCodeEnum>
    {
        public override void RaiseEvent(IEventSender sender, MovementCodeEnum code)
        {
            base.RaiseEvent(sender, code);
        }
    }
}