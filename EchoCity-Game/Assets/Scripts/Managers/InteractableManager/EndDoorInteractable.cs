using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;
        [Header("End Game Event")]
        [SerializeField] SOSwitchToGameStateEvent switchToGameStateEvent;

        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("Last door opened!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerSuccess)));
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Win, null);
            }
            else
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("Can not open door yet!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerFail)));
            }
        }
    }
}
