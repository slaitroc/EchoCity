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

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                spawnMessageEvent?.RaiseEvent("Last door opened!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(new DialogData(dialogContainerSuccess));
                switchToWinStateEvent?.RaiseEvent();
            }
            else
            {
                spawnMessageEvent?.RaiseEvent("Can not open door yet!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(new DialogData(dialogContainerFail));
            }
        }
    }
}
