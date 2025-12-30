using UnityEngine;

namespace EchoCity
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSoundEmissionDataEvent newAudioSphereEvent;
        [Header("Interactable Settings")]
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;
        protected AudioContext _audioContext;

        protected virtual void Awake() => gameObject.layer = 6; // Set to Interactable layer

        protected virtual void Start()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            Debug.Assert(puzzleManager != null, "PuzzleManager not found in the scene");
            _audioContext = new AudioContext(newAudioSphereEvent);
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
