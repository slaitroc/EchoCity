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

        protected override void ResolveInteraction(bool outcome) { }
    }
}