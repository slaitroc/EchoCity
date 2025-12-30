using System.Data;
using UnityEngine;

namespace EchoCity
{
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField] private AreaInteractable activeAreaInteractable;

        public void TriggerInteraction()
        {
            Log.DLazy(() => $"Triggered interaction on {activeAreaInteractable.name}", this);
            activeAreaInteractable?.Interact();
        }

        public void SetActiveInteractable(AreaInteractable interactable)
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
