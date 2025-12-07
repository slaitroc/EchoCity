using UnityEngine;

namespace EchoCity
{
    public class MetalBarPickable : Pickable
    {

        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainer;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        public override void InteractionOutcomeHandler(bool outcome)
        {
            base.InteractionOutcomeHandler(outcome);
            if (_waitForInteractionOutcome)
            {
                if (outcome)
                {
                    spawnMessageEvent?.RaiseEvent("Metal Bar Picked Up!", new Color(1f, 0.5f, 0f, 1f));
                    dialogDataEvent?.RaiseEvent(new DialogData(dialogContainer));
                    SetTags(setTags);
                }
            }
            _waitForInteractionOutcome = false;

        }
    }
}
