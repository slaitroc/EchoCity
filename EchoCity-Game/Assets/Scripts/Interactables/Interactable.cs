using UnityEngine;
namespace EchoCity
{
    public abstract class Interactable : MonoBehaviour, IEventSender, IInteractable, IHasDescription
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] protected SOInteractionEvent interactionEvent;
        [SerializeField] protected SOShowUIEvent showUIEvent;
        public virtual string SenderName => gameObject.name;
        public virtual int SenderID => GetInstanceID();
        public virtual bool IsManager => false;
        public virtual EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Interactable };

        [Header("Interactable Settings")]
        [SerializeField] private PuzzleManager puzzleManager;
        protected IPuzzleManager PuzzleManager => puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;

        [Header("Outcome Messages")]
        [SerializeField] protected string _interactionDescription;
        public string Description => _interactionDescription;
        public virtual bool HasRaycastDescription => true;
        public virtual bool IsInteractable => true;
        [SerializeField] protected string _InteractionSuccessMessage;
        private Color _successMessageColor = Color.green;
        protected virtual Color SuccessMessageColor => _successMessageColor;
        [SerializeField] SOQuest OnSuccessAddQuest;
        [SerializeField] SOQuest OnSuccessAddTagsQuest;
        [SerializeField] protected string _InteractionFailMessage;
        private Color _failMessageColor = Color.red;
        protected virtual Color FailMessageColor => _failMessageColor;
        [SerializeField] SOQuest OnFailAddQuest;
        [SerializeField] SOQuest OnFailAddTagsQuest;

        [SerializeField] SODialogContainer interactionContainer;

        public abstract InteractionEnum InteractionCode { get; }
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
        public virtual bool Interact()
        {
            bool outcome = puzzleManager?.TryUpdateTagsHandler(checkTags, setTags) ?? false;
            if (outcome)
                if (interactionEvent != null) interactionEvent.RaiseEvent(this, InteractionCode);
            OutcomeMessages(outcome);
            ResolveInteraction(outcome);
            AddQuest(outcome);
            return outcome;
        }

        protected abstract void ResolveInteraction(bool outcome);

        void OnValidate()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
        }

        private void OutcomeMessages(bool outcome)
        {
            InteractionLines(outcome);
            ShowPopUpMessage(outcome);
        }

        protected virtual void InteractionLines(bool outcome)
        {
            OnSuccessInteractionLine(outcome);
            OnFailInteractionLine(outcome);
        }

        protected virtual void OnFailInteractionLine(bool outcome)
        {
            if (!outcome)
            {
                if (interactionContainer)
                    if (interactionContainer.DialogLines != null && interactionContainer.DialogLines.Length > 0 && interactionContainer.DialogLines[0].AudioClip != null)
                        EchoCitySound.AddInSecondaryVoicePlayQueue(interactionContainer, showUIEvent, 0, true);
            }
        }

        protected virtual void OnSuccessInteractionLine(bool outcome)
        {
            if (outcome)
            {
                if (interactionContainer)
                    if (interactionContainer.DialogLines != null && interactionContainer.DialogLines.Length > 1 && interactionContainer.DialogLines[1].AudioClip != null)
                        EchoCitySound.AddInSecondaryVoicePlayQueue(interactionContainer, showUIEvent, 1, true);
            }
        }

        protected virtual void ShowPopUpMessage(bool outcome)
        {
            if (outcome)
            {
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionSuccessMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionSuccessMessage, SuccessMessageColor));
            }
            else
            {
                if (showUIEvent && !string.IsNullOrEmpty(_InteractionFailMessage)) showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(_InteractionFailMessage, FailMessageColor));
            }
        }
        protected virtual void DefaultAddQuest(bool outcome)
        {
            DefaultAddSuccessQuest(outcome);
            DefaultAddFailQuest(outcome);
        }

        protected virtual void DefaultAddSuccessQuest(bool outcome)
        {
            if (outcome)
            {
                if (OnSuccessAddQuest) puzzleManager.AddQuest(OnSuccessAddQuest);
                if (OnSuccessAddTagsQuest)
                {
                    puzzleManager.AddToTagsTriggeredQuests(OnSuccessAddTagsQuest);
                    puzzleManager.CheckQuests();
                }
            }
        }

        protected virtual void DefaultAddFailQuest(bool outcome)
        {
            if (!outcome)
            {
                if (OnFailAddQuest) puzzleManager.AddQuest(OnFailAddQuest);
                if (OnFailAddTagsQuest)
                {
                    puzzleManager.AddToTagsTriggeredQuests(OnFailAddTagsQuest);
                    puzzleManager.CheckQuests();
                }
            }
        }

        protected virtual void AddQuest(bool outcome)
        {
            DefaultAddQuest(outcome);
        }
    }
}