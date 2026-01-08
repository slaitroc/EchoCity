using UnityEngine;

namespace EchoCity
{
    public enum InteractionEnum
    {
        Interactable,
        Pickable
    }
    [CreateAssetMenu(fileName = "InteractionEvent", menuName = "ECHO CITY/Events/Interaction")]
    public class SOInteractionEvent : SOEventSingleParam<InteractionEnum>
    {
        public override void RaiseEvent(IEventSender sender, InteractionEnum interaction)
        {
            base.RaiseEvent(sender, interaction);
        }
    }
}