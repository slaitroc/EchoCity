using UnityEngine;

namespace EchoCity
{
    public class SecondAreaEscapeDoorInteractable : PlainInteractable
    {
        [Header("End Game Event")]
        [SerializeField] SOSwitchToGameStateEvent switchToGameStateEvent;

        public override InteractionEnum InteractionCode => InteractionEnum.SecondAreaEscapeDoor;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Win, null);
            }
            else
            {
                interactionEvent?.RaiseEvent(this, InteractionEnum.FirstAreaEscapeDoor);
            }
        }
    }
}
