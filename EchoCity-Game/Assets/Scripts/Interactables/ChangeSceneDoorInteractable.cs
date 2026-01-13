using UnityEngine;

namespace EchoCity
{
    public class ChangeSceneDoorInteractable : LinkableInteractable
    {
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private SceneEnum targetScene;
        [SerializeField] private string nextSceneName;

        private int _exitTutorialConfirmations = 0;
        private const int _requiredConfirmations = 3;

        public override InteractionEnum InteractionCode => InteractionEnum.ChangeSceneDoor;

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
            if (_exitTutorialConfirmations < _requiredConfirmations)
            {
                if (string.IsNullOrEmpty(nextSceneName))
                    nextSceneName = targetScene.ToString();
                else
                    nextSceneName = nextSceneName.Trim();
                if (outcome)
                {
                    if (showUIEvent && !string.IsNullOrEmpty(_InteractionSuccessMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams($"Interact {_requiredConfirmations - _exitTutorialConfirmations - 1} more times to reach the {nextSceneName}", SuccessMessageColor));
                }
                else
                {
                    if (showUIEvent && !string.IsNullOrEmpty(_InteractionFailMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionFailMessage, FailMessageColor));
                }
            }
            else
            {
                base.ShowPopUpMessage(outcome);
            }
        }
    }
}
