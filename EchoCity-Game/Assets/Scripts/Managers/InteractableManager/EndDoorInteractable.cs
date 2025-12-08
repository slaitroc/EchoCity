using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : Interactable
    {
        #region Constants
        protected override string _LOG_TAG => "CARD_READER";
        protected override string _TYPE_LOG_TAG => "GENERAL";
        #endregion

        [Header("End Game Event")]
        [SerializeField] SOEventVoid switchToWinStateEvent;


        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainerSuccess;
        [SerializeField] SODialogContainer dialogContainerFail;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        public override void InteractionOutcomeHandler(bool outcome)
        {
            if (_waitForInteractionOutcome)
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
            _waitForInteractionOutcome = false;
        }
    }
}
