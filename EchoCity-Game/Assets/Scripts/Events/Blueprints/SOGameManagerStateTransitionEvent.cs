using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "GameManagerStateTransitionEventSO", menuName = "ECHO CITY/Events/GameManagerStateTransitionEventSO")]
    public class SOGameManagerStateTransitionEvent : SOEventDoubleParam<GameStatesEnum, GameStatesEnum>
    {
    }
}