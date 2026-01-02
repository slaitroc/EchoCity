using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SwitchToGameState", menuName = "ECHO CITY/Events/SwitchToGameState")]
    public class SOSwitchToGameStateEvent : SOEventDoubleParam<GameStatesEnum, EventParams>
    {
        public override void RaiseEvent(IEventSender sender, GameStatesEnum gameState, EventParams eventParameter)
        {
            base.RaiseEvent(sender, gameState, eventParameter);
        }
    }

    public class ToHUDStateParams : EventParams
    {
        private readonly HudEnum _hudState;
        public HudEnum HudState => _hudState;

        public ToHUDStateParams(HudEnum hudState)
        {
            _hudState = hudState;
        }
    }

    public class ToDialogueStateParams : EventParams
    {
        private readonly DialogData _dialogData;
        public DialogData DialogData => _dialogData;

        public ToDialogueStateParams(DialogData dialogData)
        {
            _dialogData = dialogData;
        }
    }
}
