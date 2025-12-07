using System;
using EchoCity;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected string _INTERACTABLE_LOG_TAG = "INTERACTABLE";
    protected abstract string _TYPE_LOG_TAG { get; }
    protected abstract string _LOG_TAG { get; }
    protected string _LOG_TAG_FULL => $"{_TYPE_LOG_TAG}-{_INTERACTABLE_LOG_TAG}:::{_LOG_TAG}";
    protected string _LOG_COLOR = "#ff5733ff";

    [Header("Invoking Events")]
    [SerializeField] protected SOPuzzleTagEnumArrayEvent checkTagsEvent;
    [SerializeField] protected SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
    [Header("Observing Events")]
    [SerializeField] protected SOBoolEvent interactionOutcomeEvent;

    protected virtual void OnEnable()
    {
        if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised += InteractionOutcomeHandler;
        if (checkTagsEvent != null) checkTagsEvent.OnEventRaised += CheckTagsHandler;
        if (setPuzzleTagsEvent != null) setPuzzleTagsEvent.OnEventRaised += SetTagsHandler;
    }

    protected virtual void OnDisable()
    {
        if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= InteractionOutcomeHandler;
        if (checkTagsEvent != null) checkTagsEvent.OnEventRaised -= CheckTagsHandler;
        if (setPuzzleTagsEvent != null) setPuzzleTagsEvent.OnEventRaised -= SetTagsHandler;
    }

    protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer
    public abstract void Interact();
    public abstract void CheckTagsHandler(PuzzleTagEnum[] tagsToCheck);
    public abstract void SetTagsHandler(PuzzleTagEnum[] tagsToSet);
    public abstract void InteractionOutcomeHandler(bool outcome);
}
