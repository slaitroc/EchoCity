using UnityEngine;

namespace EchoCity
{
    public class MetalBarPickable : Pickable
    {

        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainer;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        protected override void ResolveInteraction(bool outcome)
        {
            base.ResolveInteraction(outcome);
            if (outcome)
            {
                spawnMessageEvent?.RaiseEvent(this, "Metal Bar Picked Up!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainer));
            }
        }
    }
}
