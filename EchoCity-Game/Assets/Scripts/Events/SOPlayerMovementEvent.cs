using UnityEngine;

namespace EchoCity
{
    public enum MovementCodeEnum
    {
        Idle,
        Move,
        Look,
        Sprint,
        Jump,
        MAX

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