using EchoCity;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IEventSender
{
    [Header("Invoking Events")]
    [SerializeField] protected SOPuzzleTagEnumArrayEvent checkTagsEvent;
    [SerializeField] protected SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
    [SerializeField] protected SOEventVoid materialToggleEvent;
    [SerializeField] protected SOSoundEmissionDataEvent newAudioSphereEvent;

    public string SenderName => gameObject.name;
    public int SenderID => GetInstanceID();
    public bool IsManager => false;
    public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Interactable };

    [Header("Observing Events")]
    [SerializeField] protected SOBoolEvent interactionOutcomeEvent;

    [Header("Interactable Settings")]
    [SerializeField] protected PuzzleTagEnum[] checkTags;
    [SerializeField] protected PuzzleTagEnum[] setTags;
    protected bool _waitForInteractionOutcome = false;
    protected AudioContext _audioContext;


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
        _audioContext = new AudioContext(this, newAudioSphereEvent);
    }
    public virtual void Interact() => CheckTags(checkTags);
    public void CheckTags(PuzzleTagEnum[] tagsToCheck)
    {
        _waitForInteractionOutcome = true;
        checkTagsEvent?.RaiseEvent(this, tagsToCheck);
    }
    public abstract void InteractionOutcomeHandler(IEventSender sender, bool outcome);
    public void SetTags(PuzzleTagEnum[] tagsToSet) => setPuzzleTagsEvent?.RaiseEvent(this, tagsToSet);
}
