using UnityEngine;
namespace EchoCity
{
    public abstract class Interactable : MonoBehaviour, IEventSender, IInteractable, IHasDescription
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSoundEmittedEvent newAudioSphereEvent;
        [Header("Interactable Settings")]
        [SerializeField] protected PuzzleManager puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;
        protected AudioContext _audioContext;
        protected Renderer[] _cachedRenderers;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Interactable };

        [SerializeField] protected string _description;
        public string Description => _description;
        public virtual bool HasRaycastDescription => true;
        public bool IsInteractable => true;

        protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer

        protected virtual void OnEnable() => _cachedRenderers = GetComponentsInChildren<Renderer>(false);

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
            _audioContext = new AudioContext(this, newAudioSphereEvent);
        }
        public virtual void Interact()
        {
            bool outcome = puzzleManager?.TryUpdateTagsHandler(checkTags, setTags) ?? false;
            ResolveInteraction(outcome);
        }

        protected abstract void ResolveInteraction(bool outcome);

        void OnValidate()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
        }
    }
}