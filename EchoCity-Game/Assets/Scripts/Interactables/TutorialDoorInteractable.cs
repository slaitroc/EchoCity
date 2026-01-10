using UnityEngine;

namespace EchoCity
{
    public class TutorialDoorInteractable : LinkableInteractable
    {
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private SceneEnum targetScene;

        private int _exitTutorialConfirmations = 0;
        private const int _requiredConfirmations = 3;

        public override InteractionEnum InteractionCode => InteractionEnum.TutorialDoor;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                if (++_exitTutorialConfirmations >= _requiredConfirmations)
                    switchLevelEvent?.RaiseEvent(this, targetScene);
            }
        }
        protected override void ShowPopUpMessage(bool outcome)
        {
            if (outcome)
            {
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionSuccessMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionSuccessMessage + $"Count: {_exitTutorialConfirmations + 1}", _successMessageColor));
            }
            else
            {
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionFailMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionFailMessage, _failMessageColor));
            }
        }
    }
}
