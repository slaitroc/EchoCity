using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractionArea : MonoBehaviour
{
    #region Constants
    private string _LOG_TAG = "INTERACTABLE RANGE";
    private string _LOG_COLOR = "#3399ffff";
    #endregion

    #region Serialized Fields
    [Header("Invoking Events")]
    [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
    [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
    [SerializeField] private AreaInteractable interactable;
    #endregion

    #region Private Fields
    #endregion

    #region Debug Fields
#pragma warning disable CS0414
    [SerializeField] private bool isPlayerInRange = false; //DEBUG
#pragma warning restore CS0414
#if UNITY_EDITOR
#pragma warning disable CS0414
    [TextArea][SerializeField] private string notes = "InteractableArea's colliders must not intersect with each other otherwise the OnTriggerEnter and OnTriggerExit events will misbehave.";
#pragma warning restore CS0414
#endif
    #endregion


    void Awake()
    {
        if (!interactable)
        {
            Log.E($"No Interactable assigned to InteractableRange on {gameObject.name}", _LOG_COLOR, _LOG_TAG);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInRange = true;
        interactable.OnEnteringRangeArea(other);
        enterInteractableAreaEvent?.RaiseEvent(interactable);

        //Log.D($"Player entered interactable range", _LOG_COLOR, _LOG_TAG);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInRange = false;
        interactable.OnExitingRangeArea(other);
        exitInteractableAreaEvent?.RaiseEvent(null);
        //Log.D($"Player exited interactable range", _LOG_COLOR, _LOG_TAG);
    }

    void OnValidate()
    {
        gameObject.layer = 6;
    }
}
