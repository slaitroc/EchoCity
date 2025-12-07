using UnityEngine;

namespace EchoCity
{
    public class InteractableManagerObserver : MonoBehaviour
    {
        [Header("Observed Events")]
        [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
        [SerializeField] private SOEventVoid interactEvent;

        [Header("GO with Handlers")]
        [SerializeField] private InteractableManager interactableManager;

        void Awake()
        {
            TryGetComponent(out interactableManager);
        }

        void OnEnable()
        {
            if (interactEvent)
            {
                interactEvent.OnEventRaised -= interactableManager.TriggerInteraction;
                interactEvent.OnEventRaised += interactableManager.TriggerInteraction;
            }
            if (enterInteractableAreaEvent)
            {
                enterInteractableAreaEvent.OnEventRaised -= interactableManager.SetActiveInteractable;
                enterInteractableAreaEvent.OnEventRaised += interactableManager.SetActiveInteractable;
            }
            if (exitInteractableAreaEvent)
            {
                exitInteractableAreaEvent.OnEventRaised -= interactableManager.SetActiveInteractable;
                exitInteractableAreaEvent.OnEventRaised += interactableManager.SetActiveInteractable;
            }
        }

        void OnDisable()
        {
            if (interactEvent) interactEvent.OnEventRaised -= interactableManager.TriggerInteraction;
            if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= interactableManager.SetActiveInteractable;
            if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= interactableManager.SetActiveInteractable;
        }
    }
}
