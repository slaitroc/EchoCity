using System.Data;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    #region Constants
    private string _LOG_TAG = "INTERACTABLE MANAGER";
    #endregion

    #region Serialized Fields
    [SerializeField] private AreaInteractable activeAreaInteractable;
    #endregion

    #region Private Fields
    #endregion

    public void TriggerInteraction()
    {
        Log.D($"Triggered interaction on {activeAreaInteractable.name}", "#33ff57ff", _LOG_TAG);
        activeAreaInteractable?.Interact();
    }

    public void SetActiveInteractable(AreaInteractable interactable)
    {
        if (interactable == null)
        {
            activeAreaInteractable = null;
            //Log.D($"Cleared active interactable", "#33ff57ff", _LOG_TAG);
            return;
        }
        activeAreaInteractable = interactable;
        //Log.D($"Set active interactable to {activeInteractable.name}", "#33ff57ff", _LOG_TAG);
    }
}
