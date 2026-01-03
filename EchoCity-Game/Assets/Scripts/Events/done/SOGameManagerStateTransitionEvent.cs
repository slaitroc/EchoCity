using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "GameManagerStateTransitionEventSO", menuName = "ECHO CITY/Events/GameManagerStateTransitionEventSO")]
    public class SOGameManagerStateTransitionEvent : SOEventDoubleParam<GameStatesEnum, GameStatesEnum>
    {
        public override void RaiseEvent(IEventSender sender, GameStatesEnum fromState, GameStatesEnum toState)
        {
            base.RaiseEvent(sender, fromState, toState);
        }
    }
}