using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "ShowUIEvent", menuName = "ECHO CITY/Events/Show UI")]
    public class SOShowUIEvent : SOEventDoubleParam<ShowableUIEnum, EventParams>
    {
        public override void RaiseEvent(IEventSender sender, ShowableUIEnum showableUI, EventParams eventParameter)
        {
            base.RaiseEvent(sender, showableUI, eventParameter);
        }
    }

    public class LoadingParams : EventParams
    {
        private bool _isLoading;
        public bool IsLoading => _isLoading;

        public LoadingParams(bool isLoading)
        {
            _isLoading = isLoading;
        }
    }

    public class DialogParams : EventParams
    {
        private readonly DialogData _dialogData;
        public DialogData DialogData => _dialogData;

        public DialogParams(DialogData dialogData)
        {
            _dialogData = dialogData;
        }
    }

    public class HudParams : EventParams
    {
        private readonly HudEnum _hudState;
        public HudEnum HudState => _hudState;

        public HudParams(HudEnum hudState)
        {
            _hudState = hudState;
        }
    }

    public class WarningParams : EventParams
    {
        private readonly string _message;
        private readonly Color _color;
        public string Message => _message;
        public Color Color => _color;

        public WarningParams(string message, Color color)
        {
            _message = message;
            _color = color;
        }
    }
}
