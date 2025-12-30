using UnityEngine;

namespace EchoCity
{
    public class BatteryPickable : Pickable
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
                spawnMessageEvent?.RaiseEvent("Battery Picked Up!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(new DialogData(dialogContainer));
            }
        }
    }
}
