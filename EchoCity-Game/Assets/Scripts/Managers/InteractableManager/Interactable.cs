using EchoCity;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IHasDescription
{
    protected string _INTERACTABLE_LOG_TAG = "INTERACTABLE";
    protected abstract string _TYPE_LOG_TAG { get; }
    protected abstract string _LOG_TAG { get; }
    protected string _LOG_TAG_FULL => $"{_TYPE_LOG_TAG}-{_INTERACTABLE_LOG_TAG}:::{_LOG_TAG}";
    protected string _LOG_COLOR = "#ff5733ff";

    [Header("Invoking Events")]
    [SerializeField] protected SOPuzzleTagEnumArrayEvent checkTagsEvent;
    [SerializeField] protected SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
    [SerializeField] protected SOEventVoid materialToggleEvent;
    [Header("Observing Events")]
    [SerializeField] protected SOBoolEvent interactionOutcomeEvent;
    [Header("Interactable Settings")]
    [SerializeField] protected PuzzleTagEnum[] checkTags;
    [SerializeField] protected PuzzleTagEnum[] setTags;
    protected bool _waitForInteractionOutcome = false;
    [SerializeField] protected string _description;
    private bool _isInteractable = true;

    public string Description => _description;
    public bool isInteractable => _isInteractable;

    protected virtual void OnEnable()
    {
        if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised += InteractionOutcomeHandler;
    }

    protected virtual void OnDisable()
    {
        if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= InteractionOutcomeHandler;
    }

    protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer
    public virtual void Interact() => CheckTags(checkTags);
    public void CheckTags(PuzzleTagEnum[] tagsToCheck)
    {
        _waitForInteractionOutcome = true;
        checkTagsEvent?.RaiseEvent(tagsToCheck);
    }
    public abstract void InteractionOutcomeHandler(bool outcome);
    public void SetTags(PuzzleTagEnum[] tagsToSet) => setPuzzleTagsEvent?.RaiseEvent(tagsToSet);
}
