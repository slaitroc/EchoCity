using System;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTrigger : MonoBehaviour, IEventSender
    {
        [SerializeField] protected SODialogContainer tutorialDialogContainer;
        [SerializeField] protected SOSwitchToGameStateEvent switchToNarrationStateEvent;
        [Header("Puzzle Tags")]
        [SerializeField] protected PuzzleManager puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] {
            EventSenderCategoriesEnum.Puzzle,
            EventSenderCategoriesEnum.Tutorial
        };

        void OnEnable()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            Debug.Assert(puzzleManager != null, "PuzzleManager not found in the scene");
        }
        protected virtual void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                switchToNarrationStateEvent?.RaiseEvent(this, GameStatesEnum.Narration, new ToDialogueStateParams(new DialogData(tutorialDialogContainer.DialogLines)));
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            ResolveInteraction(puzzleManager?.TryUpdateTagsHandler(checkTags, setTags) ?? false);
        }

        void OnValidate()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
        }
    }
}
