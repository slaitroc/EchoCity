using UnityEngine;

namespace EchoCity
{

    public enum InteractionsEnum
    {
        EndDoor,
        CardReader,
        Generator,
    }

    [CreateAssetMenu(fileName = "SOInteractionEvent", menuName = "ECHO CITY/Events/Interaction")]
    public class SOInteractionEvent : SOSingleParamEvent<InteractionsEnum>
    {
        public override void RaiseEvent(IEventSender sender, InteractionsEnum interaction)
        {
            base.RaiseEvent(sender, interaction);
        }
    }
}