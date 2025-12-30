using EchoCity;
using UnityEngine;

namespace EchoCity
{

    [RequireComponent(typeof(Collider))]
    public class InteractableFixDetector : Interactable
    {
        [SerializeField] private Interactable linkedInteractable;

        public override void Interact()
        {
            base.Interact();
            linkedInteractable?.Interact();
        }

        public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            _waitForInteractionOutcome = false;
        }
    }
}