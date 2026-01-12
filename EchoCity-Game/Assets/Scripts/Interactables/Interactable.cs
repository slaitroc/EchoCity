using UnityEngine;
namespace EchoCity
{
    public abstract class Interactable : MonoBehaviour, IEventSender, IInteractable, IHasDescription
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] protected SOInteractionEvent interactionEvent;
        [SerializeField] protected SOShowUIEvent showUIEvent;
        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Interactable };

        [Header("Interactable Settings")]
        [SerializeField] protected PuzzleManager puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;

        [Header("Outcome Messages")]
        [SerializeField] protected string _interactionDescription;
        public string Description => _interactionDescription;
        public virtual bool HasRaycastDescription => true;
        public bool IsInteractable => true;
        [SerializeField] protected string _InteractionSuccessMessage;
        private Color _successMessageColor = Color.green;
        [SerializeField] protected string _InteractionFailMessage;
        private Color _failMessageColor = Color.red;
        [SerializeField] SODialogContainer interactionContainer;

        protected virtual InteractionEnum _interactionCode { get; }
        protected AudioContext _audioContext;
        protected Renderer[] _cachedRenderers;

        protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer

        protected virtual void OnEnable()
        {
            _cachedRenderers = GetComponentsInChildren<Renderer>(false);
            if (_cachedRenderers == null) return;
            for (int i = 0; i < _cachedRenderers.Length; i++)
            {
                var r = _cachedRenderers[i];
                if (r != null)
                    r.gameObject.layer = gameObject.layer;
            }
        }

        protected virtual void OnDisable()
        {
            if (_cachedRenderers != null)
                InteractableOutlineRenderer.Unregister(_cachedRenderers);
        }

        protected virtual void Start()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            Debug.Assert(puzzleManager != null, "PuzzleManager not found in the scene");
            _audioContext = new AudioContext(this, soundEmittedEvent);
        }
        public virtual void Interact()
        {
            bool outcome = puzzleManager?.TryUpdateTagsHandler(checkTags, setTags) ?? false;
            if (outcome)
                if (interactionEvent != null) interactionEvent.RaiseEvent(this, _interactionCode);
            OutcomeMessages(outcome);
            ResolveInteraction(outcome);
        }

        protected abstract void ResolveInteraction(bool outcome);

        void OnValidate()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
        }

        private void OutcomeMessages(bool outcome)
        {
            if (outcome)
            {
                if (interactionContainer)
                    EchoCitySound.AddInSecondaryVoicePlayQueue(interactionContainer, showUIEvent, 1, true);
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionSuccessMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionSuccessMessage, _successMessageColor));
            }
            else
            {
                if (interactionContainer)
                    EchoCitySound.AddInSecondaryVoicePlayQueue(interactionContainer, showUIEvent, 0, true);
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionFailMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionFailMessage, _failMessageColor));
            }
        }
    }
}