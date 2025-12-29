using System.Data;
using UnityEngine;

namespace EchoCity
{
    public class InteractableManager : MonoBehaviour
    {
        private string _LOG_TAG = "INTERACTABLE MANAGER";
        private string _LOG_COLOR = "#33ff57ff";

        [SerializeField] private AreaInteractable activeAreaInteractable;

        public void TriggerInteraction()
        {
            Log.DLazy(() => $"Triggered interaction on {activeAreaInteractable.name}", _LOG_TAG, _LOG_COLOR);
            activeAreaInteractable?.Interact();
        }

        public void SetActiveInteractable(AreaInteractable interactable)
        {
            if (interactable == null)
            {
                activeAreaInteractable = null;
                Log.DLazy(() => $"Cleared active interactable", _LOG_TAG, _LOG_COLOR);
                return;
            }
            activeAreaInteractable = interactable;
            Log.DLazy(() => $"Set active interactable to {activeAreaInteractable.name}", _LOG_TAG, _LOG_COLOR);
        }
    }
}
