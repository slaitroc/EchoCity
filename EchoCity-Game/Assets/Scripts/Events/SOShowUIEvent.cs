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

    public class NarrationParams : EventParams
    {
        private readonly SODialogContainer _narrationContainer;
        private readonly SceneEnum _destinationScene;
        private readonly bool _useCached;
        public SODialogContainer NarrationContainer => _narrationContainer;
        public SceneEnum DestinationScene => _destinationScene;
        public bool UseCached => _useCached;

        public NarrationParams(SODialogContainer dialogContainer, SceneEnum destinationScene, bool useCached = false)
        {
            _useCached = useCached;
            _narrationContainer = dialogContainer;
            _destinationScene = destinationScene;
        }

        public NarrationParams(ToNarrationParams toNarrationParams, bool useCached = false)
        {
            _useCached = useCached;
            _narrationContainer = toNarrationParams.NarrationContainer;
            _destinationScene = toNarrationParams.DestinationScene;
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

    public class PopUpMessageParams : EventParams
    {
        private readonly string _message;
        private readonly Color _color;
        public string Message => _message;
        public Color Color => _color;

        public PopUpMessageParams(string message, Color color)
        {
            _message = message;
            _color = color;
        }
    }

    public class SubtitleParams : EventParams
    {
        private readonly string _speakerName;
        private readonly string _subtitle;
        private readonly float _duration;

        public string SpeakerName => _speakerName;
        public string Subtitle => _subtitle;
        public float Duration => _duration;

        public SubtitleParams(string speakerName, string subtitle, float duration)
        {
            _speakerName = speakerName;
            _subtitle = subtitle;
            _duration = duration;
        }
    }


}
