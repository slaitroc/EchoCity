using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : Interactable
    {
        [Header("End Game Event")]
        [SerializeField] SOEventVoid switchToWinStateEvent;


        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainerSuccess;
        [SerializeField] SODialogContainer dialogContainerFail;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            if (_waitForInteractionOutcome)
            {
                if (outcome)
                {
                    spawnMessageEvent?.RaiseEvent(this, "Last door opened!", new Color(1f, 0.5f, 0f, 1f));
                    dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainerSuccess));
                    switchToWinStateEvent?.RaiseEvent(this);
                }
                else
                {
                    spawnMessageEvent?.RaiseEvent(this, "Can not open door yet!", new Color(1f, 0.5f, 0f, 1f));
                    dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainerFail));
                }
            }
            _waitForInteractionOutcome = false;
        }
    }
}
