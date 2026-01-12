using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class InteractableFixDetector : PlainInteractable
    {
        [SerializeField] private Interactable linkedInteractable;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                linkedInteractable.Interact();
            }
        }
    }
}