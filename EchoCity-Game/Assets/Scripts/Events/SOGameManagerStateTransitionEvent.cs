using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "GameManagerStateTransitionEvent", menuName = "ECHO CITY/Events/Game Manager State Transition")]
    public class SOGameManagerStateTransitionEvent : SOEventDoubleParam<GameStatesEnum, GameStatesEnum>
    {
        public override void RaiseEvent(IEventSender sender, GameStatesEnum fromState, GameStatesEnum toState)
        {
            base.RaiseEvent(sender, fromState, toState);
        }
    }
}