using System.Data;
using UnityEngine;

namespace EchoCity
{
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField] private AreaInteractable activeAreaInteractable;

        [Header("Observed Events")]
        [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
        [SerializeField] private SOEventVoid interactEvent;

        void OnEnable()
        {
            if (interactEvent) interactEvent.OnEventRaised += TriggerInteraction;
            if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised += SetActiveInteractable;
            if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised += SetActiveInteractable;
        }

        void OnDisable()
        {
            if (interactEvent) interactEvent.OnEventRaised -= TriggerInteraction;
            if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= SetActiveInteractable;
            if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= SetActiveInteractable;
        }
        public void TriggerInteraction(IEventSender sender)
        {
            Log.DLazy(() => $"Triggered interaction on {activeAreaInteractable.name}", this);
            activeAreaInteractable?.Interact();
        }

        public void SetActiveInteractable(IEventSender sender, AreaInteractable interactable)
        {
            if (interactable == null)
            {
                activeAreaInteractable = null;
                Log.DLazy(() => $"Cleared active interactable", this);
                return;
            }
            activeAreaInteractable = interactable;
            Log.DLazy(() => $"Set active interactable to {activeAreaInteractable.name}", this);
        }
    }
}
