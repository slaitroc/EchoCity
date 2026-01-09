using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;
        [SerializeField] protected SOInteractionEvent interactionEvent;
        [Header("End Game Event")]
        [SerializeField] SOSwitchToGameStateEvent switchToGameStateEvent;

        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Win, null);
            }
        }
    }
}
