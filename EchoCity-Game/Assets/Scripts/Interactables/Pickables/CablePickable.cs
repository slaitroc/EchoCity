using UnityEngine;

namespace EchoCity
{
    public class CablePickable : Pickable
    {
        protected override void ResolveInteraction(bool outcome)
        {
            base.ResolveInteraction(outcome);
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("Cable Picked Up!", new Color(1f, 0.5f, 0f, 1f)));
            }
        }
    }
}
