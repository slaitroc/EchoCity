using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class InteractableFixDetector : PlainInteractable
    {
        [SerializeField] private Interactable linkedInteractable;

        public override InteractionEnum InteractionCode => linkedInteractable.InteractionCode;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                linkedInteractable.Interact();
            }
        }
    }
}