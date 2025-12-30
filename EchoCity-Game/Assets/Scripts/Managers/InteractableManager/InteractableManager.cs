using System.Data;
using UnityEngine;

namespace EchoCity
{
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField] private AreaInteractable activeAreaInteractable;

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
