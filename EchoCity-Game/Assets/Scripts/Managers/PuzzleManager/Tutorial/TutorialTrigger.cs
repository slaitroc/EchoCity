using System;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTrigger : MonoBehaviour
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

        void OnEnable()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised += InteractionOutcomeHandler;
        }

        void OnDisable()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= InteractionOutcomeHandler;
        }

        private void InteractionOutcomeHandler(bool outcome)
        {
            if (!outcome) return;
            if (outcome)
                setTagsEvent?.RaiseEvent(setTags);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            checkTagsEvent?.RaiseEvent(checkTags);
            switchToNarrationStateEvent?.RaiseEvent(new DialogData(tutorialDialogContainer));
            gameObject.SetActive(false);
        }
    }
}
