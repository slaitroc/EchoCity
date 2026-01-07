using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SwitchToGameStateEvent", menuName = "ECHO CITY/Events/Switch To Game State")]
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

    public class ToNarrationParams : EventParams
    {
        private readonly SODialogContainer _dialogData;
        public SODialogContainer DialogData => _dialogData;

        public ToNarrationParams(SODialogContainer dialogData)
        {
            _dialogData = dialogData;
        }
    }

    public class ToLoadingStateParams : EventParams
    {
        private readonly bool _isLoading;
        public bool IsLoading => _isLoading;

        public ToLoadingStateParams(bool isLoading)
        {
            _isLoading = isLoading;
        }
    }
}
