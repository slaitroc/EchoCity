using System;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTrigger : MonoBehaviour, IEventSender
    {
        [SerializeField] protected SODialogContainer tutorialDialogContainer;
        [SerializeField] protected SODialogDataEvent switchToNarrationStateEvent;
        [Header("Invoking Events")]
        [SerializeField] protected SOPuzzleTagEnumArrayEvent checkTagsEvent;
        [SerializeField] protected SOPuzzleTagEnumArrayEvent setTagsEvent;
        [Header("Observing Events")]
        [SerializeField] protected SOBoolEvent interactionOutcomeEvent;
        [Header("Puzzle Tags")]
        [SerializeField] protected PuzzleTagEnum[] checkTags;
        [SerializeField] protected PuzzleTagEnum[] setTags;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] {
            EventSenderCategoriesEnum.Puzzle,
            EventSenderCategoriesEnum.Tutorial
        };

        void OnEnable()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised += InteractionOutcomeHandler;
        }

        void OnDisable()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= InteractionOutcomeHandler;
        }

        private void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            if (!outcome) return;
            if (outcome)
                setTagsEvent?.RaiseEvent(this, setTags);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            checkTagsEvent?.RaiseEvent(this, checkTags);
            switchToNarrationStateEvent?.RaiseEvent(this, new DialogData(tutorialDialogContainer));
            gameObject.SetActive(false);
        }
    }
}
