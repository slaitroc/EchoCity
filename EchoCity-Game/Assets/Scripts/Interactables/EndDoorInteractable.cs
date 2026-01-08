using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;
        [Header("End Game Event")]
        [SerializeField] SOEventVoid switchToWinStateEvent;

        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("Last door opened!", new Color(1f, 0.5f, 0f, 1f)));
                // showUIEvent?.RaiseEvent(this, ShowableUIEnum.Narration, new NarrationParams(new DialogData(dialogContainerSuccess)));
                switchToWinStateEvent?.RaiseEvent(this);
            }
            else
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("Can not open door yet!", new Color(1f, 0.5f, 0f, 1f)));
                // showUIEvent?.RaiseEvent(this, ShowableUIEnum.Narration, new NarrationParams(new DialogData(dialogContainerFail)));
            }
        }
    }
}
