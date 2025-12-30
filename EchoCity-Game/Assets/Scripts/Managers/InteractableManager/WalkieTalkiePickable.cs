using UnityEngine;

namespace EchoCity
{
    public class WalkieTalkiePickable : Pickable
    {


        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainer;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            base.InteractionOutcomeHandler(sender, outcome);
            if (_waitForInteractionOutcome)
            {
                if (outcome)
                {
                    spawnMessageEvent?.RaiseEvent(this, "WalkieTalkie Picked Up!", new Color(1f, 0.5f, 0f, 1f));
                    dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainer));
                    SetTags(setTags);
                }
            }
            _waitForInteractionOutcome = false;
        }
    }
}
