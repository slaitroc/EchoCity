using UnityEngine;

namespace EchoCity
{
    public class FloppyDiskPickable : Pickable
    {
        protected override void ResolveInteraction(bool outcome)
        {
            base.ResolveInteraction(outcome);
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("FloppyDisk Picked Up!", new Color(1f, 0.5f, 0f, 1f)));
            }
        }
    }
}
