using UnityEngine;

namespace EchoCity
{
    public class MetalBarPickable : Pickable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;
        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainer;

        protected override void ResolveInteraction(bool outcome)
        {
            base.ResolveInteraction(outcome);
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("Metal Bar Picked Up!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainer)));
            }
        }
    }
}
