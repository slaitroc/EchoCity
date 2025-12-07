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
    [Header("Interactable Settings")]
    [SerializeField] protected virtual PuzzleTagEnum[] checkTags { get; set; }
    [SerializeField] protected virtual PuzzleTagEnum[] setTags { get; set; }

    protected virtual void OnEnable()
    {
        if (setPuzzleTagsEvent != null) setPuzzleTagsEvent.OnEventRaised += SetTags;
    }

    protected virtual void OnDisable()
    {
        if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= InteractionOutcomeHandler;
    }

    protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer
    public virtual void Interact() => CheckTags(checkTags);
    public void CheckTags(PuzzleTagEnum[] tagsToCheck) => checkTagsEvent?.RaiseEvent(tagsToCheck);
    public virtual void InteractionOutcomeHandler(bool outcome)
    {
        if (outcome)
            SetTags(setTags);
    }
    public void SetTags(PuzzleTagEnum[] tagsToSet) => setPuzzleTagsEvent?.RaiseEvent(tagsToSet);
}
