using EchoCity;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IHasDescription
{
    [Header("Invoking Events")]
    [SerializeField] protected SOPuzzleTagEnumArrayEvent checkTagsEvent;
    [SerializeField] protected SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
    [SerializeField] protected SOEventVoid materialToggleEvent;
    [SerializeField] protected SOSoundEmissionDataEvent newAudioSphereEvent;
    [Header("Observing Events")]
    [SerializeField] protected SOBoolEvent interactionOutcomeEvent;
    [Header("Interactable Settings")]
    [SerializeField] protected PuzzleTagEnum[] checkTags;
    [SerializeField] protected PuzzleTagEnum[] setTags;

    protected bool _waitForInteractionOutcome = false;
    protected AudioContext _audioContext;

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

    protected virtual void Start()
    {
        _audioContext = new AudioContext(newAudioSphereEvent);
    }
    public virtual void Interact() => CheckTags(checkTags);
    public void CheckTags(PuzzleTagEnum[] tagsToCheck)
    {
        _waitForInteractionOutcome = true;
        checkTagsEvent?.RaiseEvent(tagsToCheck);
    }
    public abstract void InteractionOutcomeHandler(bool outcome);
    public void SetTags(PuzzleTagEnum[] tagsToSet) => setPuzzleTagsEvent?.RaiseEvent(tagsToSet);
}
