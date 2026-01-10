using UnityEngine;

namespace EchoCity
{
    public class EndDoorInteractable : PlainInteractable
    {
        [Header("End Game Event")]
        [SerializeField] SOSwitchToGameStateEvent switchToGameStateEvent;

        public override InteractionEnum InteractionCode => InteractionEnum.EndDoor;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Win, null);
            }
            else
            {
                interactionEvent?.RaiseEvent(this, InteractionEnum.EndDoor);
            }
        }
    }
}
